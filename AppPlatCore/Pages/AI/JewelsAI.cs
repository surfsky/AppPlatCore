﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data;
using App.Utils;
using App.Web;

namespace App.Pages.AI
{
    /// <summary>
    /// 输入模型类
    /// </summary>
    public class ModelInput
    {
        [LoadColumn(0)]
        [ColumnName(@"Label")]
        public string Label { get; set; }

        [LoadColumn(1)]
        [ColumnName(@"ImageSource")]
        public byte[] ImageSource { get; set; }

    }


    /// <summary>
    /// 输出模型类
    /// </summary>
    public class ModelOutput
    {
        [ColumnName(@"Label")]
        public uint Label { get; set; }

        [ColumnName(@"ImageSource")]
        public byte[] ImageSource { get; set; }

        [ColumnName(@"PredictedLabel")]
        public string PredictedLabel { get; set; }

        [ColumnName(@"Score")]
        public float[] Score { get; set; }

    }

    /// <summary>
    /// 预测结果
    /// </summary>
    public class PredicateResult
    {
        public string Label { get; set; }
        public float Score { get; set; }
        public PredicateResult(string label, float score)
        {
            Label = label;
            Score = score;
        }
    }

    /// <summary>
    /// 图像识别 AI
    /// </summary>
    public class JewelsAI
    {
        /// <summary>训练目录</summary>
        public static string TrainPath = "/Pages/AI/JewelsImages/";
        /// <summary>模型文件路径</summary>
        public static string ModelPath = "/Pages/AI/JewelsImages/model.zip";

        //---------------------------------------------------------
        // 引擎
        //---------------------------------------------------------
        //public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);
        static PredictionEngine<ModelInput, ModelOutput> _engine;
        public static PredictionEngine<ModelInput, ModelOutput> Engine
        {
            get
            {
                if (_engine == null)
                    _engine = CreatePredictEngine();
                return _engine;
            }
        }

        public static void ReloadEngine()
        {
            _engine = CreatePredictEngine();
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var physicalPath = Asp.MapPath(ModelPath);
            var context = new MLContext();
            ITransformer transformer = context.Model.Load(physicalPath, out var _);
            return context.Model.CreatePredictionEngine<ModelInput, ModelOutput>(transformer);
        }

        //---------------------------------------------------------
        // 训练
        //---------------------------------------------------------
        /// <summary>Load an IDataView from a folder path.</summary>
        /// <param name="mlContext">The common context for all ML.NET operations.</param>
        /// <param name="folder"> Folder to the image data for training.</param>
        public static IDataView LoadImageFromFolder(MLContext mlContext, string folder)
        {
            var res = new List<ModelInput>();
            var allowedImageExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folder);
            DirectoryInfo[] subDirectories = rootDirectoryInfo.GetDirectories();

            if (subDirectories.Length == 0)
            {
                throw new Exception("fail to find subdirectories");
            }

            foreach (DirectoryInfo directory in subDirectories)
            {
                var imageList = directory.EnumerateFiles().Where(f => allowedImageExtensions.Contains(f.Extension.ToLower()));
                if (imageList.Count() > 0)
                {
                    res.AddRange(imageList.Select(i => new ModelInput
                    {
                        Label = directory.Name,
                        ImageSource = File.ReadAllBytes(i.FullName),
                    }));
                }
            }
            return mlContext.Data.LoadFromEnumerable(res);
        }

        /// <summary>Retrain model using the pipeline generated as part of the training process.</summary>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            return model;
        }

        /// <summary>build the pipeline that is used from model builder. Use this function to retrain model.</summary>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: @"Label", inputColumnName: @"Label", addKeyValueAnnotationsAsText: false)
                                    .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(labelColumnName: @"Label", scoreColumnName: @"Score", featureColumnName: @"ImageSource"))
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: @"PredictedLabel", inputColumnName: @"PredictedLabel"));

            return pipeline;
        }

        /// <summary>build model.</summary>
        public static void BuildModel()
        {
            var trainPath = Asp.MapPath(TrainPath);
            var modelPath = Asp.MapPath(ModelPath);
            var context = new MLContext();
            var trainData = LoadImageFromFolder(context, trainPath);
            var pipeline = BuildPipeline(context);
            var transformer = pipeline.Fit(trainData);
            context.Model.Save(transformer, trainData.Schema, modelPath);
        }


        //---------------------------------------------------------
        // 预测
        //---------------------------------------------------------
        /// <summary>Use this method to predict scores for all possible labels.</summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static List<PredicateResult> PredictAllLabels(ModelInput input)
        {
            var predEngine = Engine;// PredictEngine.Value;
            var result = predEngine.Predict(input);
            return GetSortedScoresWithLabels(result).Cast(t => new PredicateResult(t.Key, t.Value));
        }

        /// <summary>Map the unlabeled result score array to the predicted label names.</summary>
        /// <param name="result">Prediction to get the labeled scores from.</param>
        /// <returns>Ordered list of label and score.</returns>
        /// <exception cref="Exception"></exception>
        public static IOrderedEnumerable<KeyValuePair<string, float>> GetSortedScoresWithLabels(ModelOutput result)
        {
            var unlabeledScores = result.Score;
            var labelNames = GetLabels(result);

            Dictionary<string, float> labledScores = new Dictionary<string, float>();
            for (int i = 0; i < labelNames.Count(); i++)
            {
                // Map the names to the predicted result score array
                var labelName = labelNames.ElementAt(i);
                labledScores.Add(labelName.ToString(), unlabeledScores[i]);
            }

            return labledScores.OrderByDescending(c => c.Value);
        }

        /// <summary>Get the ordered label names.</summary>
        /// <param name="result">Predicted result to get the labels from.</param>
        /// <returns>List of labels.</returns>
        /// <exception cref="Exception"></exception>
        private static IEnumerable<string> GetLabels(ModelOutput result)
        {
            var schema = Engine.OutputSchema; // PredictEngine.Value.OutputSchema;

            var labelColumn = schema.GetColumnOrNull("Label");
            if (labelColumn == null)
            {
                throw new Exception("Label column not found. Make sure the name searched for matches the name in the schema.");
            }

            // Key values contains an ordered array of the possible labels. This allows us to map the results to the correct label value.
            var keyNames = new VBuffer<ReadOnlyMemory<char>>();
            labelColumn.Value.GetKeyValues(ref keyNames);
            return keyNames.DenseValues().Select(x => x.ToString());
        }

        /// <summary>Use this method to predict on <see cref="ModelInput"/>.</summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = Engine; // PredictEngine.Value;
            return predEngine.Predict(input);
        }


    }
}

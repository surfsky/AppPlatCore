using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Entities;

namespace App.DAL
{
    public class Online : EntityBase<Online>
    {
        [Display(Name = "IP地址")]
        [StringLength(50)]
        public string IPAdddress { get; set; }

        [Display(Name = "登录时间")]
        public DateTime LoginTime { get; set; }

        [Display(Name = "最后操作时间")]
        public DateTime? UpdateTime { get; set; }




        [Display(Name = "用户")]
        [Required]
        public long UserID { get; set; }
        public User User { get; set; }


    }
}
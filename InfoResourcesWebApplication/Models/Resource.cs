using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace InfoResourcesWebApplication
{
    [Table("Resources")]
    public partial class Resource
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceId { get; set; }

        [Display(Name = "Назва ресурсу")]
        public string ResourceName { get; set; }

        private string _urlAdress;
        [Display(Name = "URL")]
        public string UrlAddress
        {
            get { return _urlAdress; }
            set
            {
                if (value != null)
                {
                    _urlAdress = Uri.EscapeUriString(value);
                }
                else
                {
                    _urlAdress = null;
                }

            }
        }

        [Display(Name = "Тип ресурсу")]
        public int Type { get; set; }
        [Display(Name = "Автор")]
        public int Author { get; set; }
        [Display(Name = "Коротка аннотація")]
        public string Annotation { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddDate { get; set; }

        [ForeignKey("Author")]
        public virtual Author AuthorNavigation { get; set; }
        [ForeignKey("Type")]
        public virtual ResourceType TypeNavigation { get; set; }
    }
}

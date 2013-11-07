using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Persistence;

namespace Umbraco.VS.NewProject.Wizard.WPF
{
    [TableName("umbracoUser")]
    [PrimaryKey("id", autoIncrement = true)]
    public class UserDto
    {
        [Column("id")]
        public int Id { get; set; }

    }
}

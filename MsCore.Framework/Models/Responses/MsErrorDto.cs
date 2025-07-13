using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsCore.Framework.Models.Responses
{
    public class MsErrorDto
    {
        public List<string> Errors { get; set; }
        public bool IsShow { get; set; }

        public MsErrorDto(List<string> errors,bool isShow = true)
        {
            Errors = errors ?? new List<string>();
            IsShow = isShow;
        }

        public MsErrorDto(string error,bool isShow = true)
        {
            Errors = new List<string> { error };
            IsShow = isShow;
        }
    }
}

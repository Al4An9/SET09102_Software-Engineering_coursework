using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBMFS.Models;
using System.IO;
using Newtonsoft.Json;
using NBMFS.Views;

namespace NBMFS.Database
{
    public class SaveToFile
    {
        public string ErrorCode { get; set; }

        public List<Sms> LoadJsonSms()
        {
            string data = File.ReadAllText("sms.json");
            return JsonConvert.DeserializeObject<List<Sms>>(data) ?? new List<Sms>();
        }

        public List<Tweet> LoadJsonTweet()
        {
            string data = File.ReadAllText("tweet.json");
            return JsonConvert.DeserializeObject<List<Tweet>>(data) ?? new List<Tweet>();
        }

        public List<Email> LoadJsonEmail()
        {
            string data = File.ReadAllText("email.json");
            return JsonConvert.DeserializeObject<List<Email>>(data) ?? new List<Email>();
        }
        public List<SIR> LoadJsonSir()
        {
            string data = File.ReadAllText("sir.json");
            return JsonConvert.DeserializeObject<List<SIR>>(data) ?? new List<SIR>();
        }
    }
}

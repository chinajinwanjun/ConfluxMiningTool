using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace ConfluxMiningTool.Models
{
    public class MyHttpClient
    {
        public readonly  HttpClient _client;

        public MyHttpClient(HttpClient client)
        {
            _client = client;
        }
    }
}

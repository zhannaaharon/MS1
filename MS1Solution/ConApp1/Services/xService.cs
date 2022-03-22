using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConApp1.Services
{
    public class XService:IService
    {

        private readonly ILogger<IService> _log;
        private readonly IConfiguration _config;


        // this class has to DEMAND its dependencies

        public XService(ILogger<XService> log, IConfiguration config)
        {
            _log = log;
            _config = config;
            // _http = http;

            //reqCounter = 0;

            // Read from configuration file
            //ReadConfiguration();


            //_GoldLineServiceUrl = "https://jsonplaceholder.typicode.com/posts";

        }



        // to replace what is done in Main()
        public void Run()
        {
            int LoopCounter = _config.GetValue<int>("LoopTimes");
            int i = 0;

            // user logger and read 10 from configuration
            while (true)
            {
                /* Structural logger as Serilog */
                _log.LogInformation(" Run number {runNumber} out of {LoopCounter}", i , LoopCounter);
                Thread.Sleep(100);
                i++;
            }

        }


    }




}

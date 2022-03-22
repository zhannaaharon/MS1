using ConApp1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

// goal - access secret file from openshift S

namespace ConApp1 // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // because we want log instantly we need a manual connection to appsettings.json.
            
            /* setup the builder for configuration*/
            var _builder = new ConfigurationBuilder();

            // reference of _builder is passed
            BuildConfig(_builder);


            /*setup a logger from Serilog               */
            /* all functions called after new (except CreateLogger) return LoggerConfiguration object 
             * which allows to CHAIN them one after another */

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(_builder.Build()) // returns IConfigurationRoot => allows logger to read the config from appsettings.json 
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();


            Log.Logger.Information("==== Running INGEST === ");


            /* host object has everything for dependency injection , configuration appsettings.json and logging !!!*/

            var host = Host.CreateDefaultBuilder()
                // configuration of all dependency injections 
                .ConfigureServices((contex, services) =>
                {

                    // in general , the console app exits  ( If Console.ReadKey() is not used  ),
                    // So activate a class doing something with .Run() method which will be activated when the console Main is done
                    // We do not need it really because we have the Timer callback but just to keep the option open...
                    //
                    // AddTransient means give me an instance every time I ask for it based on Interface class 
                    // while interface class has to have a Run() method

                    services.AddTransient<IService, XService>();

                }

                ).UseSerilog() //overrides ILoggerFactory behaviour
                .Build();

            /* this is the command to host to find the XService INSTANCE in its services */
            var svc = ActivatorUtilities.CreateInstance<XService>(host.Services);

            //svcRef = svc;

            svc.Run();

        }


        /* this function happens BEFORE the setup and this will allow to do logging before we work with the configuration*/
        private static void BuildConfig(IConfigurationBuilder builder)
        {


            // directive to a builder to talk to appsettings.json in the same directory where <>.exe uis running
            // the second Add is using a STRING INTERPOLATION to add env dependent json

            // the ASPNETCORE_ENVIRONMENT env variable can have the following values :
            // Development: set by  launchSettings.json file sets ASPNETCORE_ENVIRONMENT to Development on the local machine.
            // Production: The default if DOTNET_ENVIRONMENT and ASPNETCORE_ENVIRONMENT have not been set.
            // "??" means that in case the appsettings.Development.json is not found - the production one is used
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();


        }
    }
}

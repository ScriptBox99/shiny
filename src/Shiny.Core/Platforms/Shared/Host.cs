﻿using System;
using Microsoft.Extensions.Logging;

namespace Shiny.Hosting;


public abstract partial class Host : IHost
{
    const string InitFailErrorMessage = "ServiceProvider is not initialized - This means you have not setup Shiny correctly!  Please follow instructions at https://shinylib.net";


    static IHost? currentHost;
    public static IHost Current
    {
        get
        {
            if (currentHost == null)
                throw new InvalidOperationException(InitFailErrorMessage);

            return currentHost;
        }
        internal set
        {
            ArgumentNullException.ThrowIfNull(value);
            currentHost = value;
        }
    }

    public static bool IsInitialized => currentHost != null;


    protected Host(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        this.ServiceProvider = serviceProvider;
        this.Logging = loggerFactory;
    }


    public IServiceProvider ServiceProvider { get; init; }
    public ILoggerFactory Logging { get; init; }
}


public partial class Host
{
    public static IHostBuilder CreateDefaultBuilder()
#if ANDROID
        => new AndroidHostBuilder();    
#elif IOS
        => new IosHostBuilder();
#else
        => throw new InvalidProgramException("Invalid Shiny Platform");
#endif
}
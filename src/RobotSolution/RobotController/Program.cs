// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using RobotController;
using RobotController.Extensions;


internal class Program
{
    static StateMonitor stateMonitor;
    static InputsCommandTransmitter inputTransmitter;
    private static async Task Main(string[] args)
    {
        using CancellationTokenSource cts = new();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            cts.Cancel();
            eventArgs.Cancel = true; 
        };
        App.ServiceProvider = ConfigureServices();

        inputTransmitter = new InputsCommandTransmitter();
        inputTransmitter.SetEvents();

        stateMonitor = new StateMonitor();
        stateMonitor.StopRobot += (s, e) =>
        {
            inputTransmitter.Stop();
        };

        _ = Task.Run(() => stateMonitor.SendStatusAndWatchCommunication(cts.Token));

        try
        {
            await Task.Delay(Timeout.Infinite, cts.Token);
        }
        catch (TaskCanceledException)
        {
            
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRobotControllerServices();
        return serviceCollection.BuildServiceProvider();
    }
}
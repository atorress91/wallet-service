using Microsoft.Extensions.DependencyInjection;

namespace WalletService.Core.Kafka.Topics;

public static class KafkaTopics
{
    public static string ProcessModel1ATopic { get; set; } = "process-model-1A";
    public static string ProcessModel1BTopic { get; set; } = "process-model-1B";
    public static string ProcessModel2Topic { get; set; } = "process-model-2";
    public static string ProcessPaymentModelTwoThreeTopic { get; set; } = "process-payment";
    public static string ProcessModelFourFiveSixTopic { get; set; } = "process-model-four";
    public static string ProcessModel3Topic { get; set; } = "process-model-3";

    public static void RegisterKafkaTopics(this IServiceCollection services, string env)
    {
        var envLower = env.ToLower();
        ProcessModel2Topic     += "-" + envLower;
        ProcessPaymentModelTwoThreeTopic += "-" + envLower;
        ProcessModel3Topic             += "-" + envLower;
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace WalletService.Core.Kafka.Topics;

public static class KafkaTopics
{
    public static string ProcessModelThreeWithOutTopic { get; set; } = "ecoPool-out";
    public static string ProcessModelThreeWithInTopic { get; set; } = "ecoPool-in";
    public static string ProcessPaymentModelTwoThreeTopic { get; set; } = "process-payment";
    public static string ProcessModelFourFiveSixTopic { get; set; } = "process-model-four";
    public static string ProcessModelTwoTopic { get; set; } = "process-model-two";

    public static void RegisterKafkaTopics(this IServiceCollection services, string env)
    {
        var envLower = env.ToLower();
        ProcessModelThreeWithOutTopic    += "-" + envLower;
        ProcessModelThreeWithInTopic     += "-" + envLower;
        ProcessPaymentModelTwoThreeTopic += "-" + envLower;
        ProcessModelTwoTopic             += "-" + envLower;
    }
}
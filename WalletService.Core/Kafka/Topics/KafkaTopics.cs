using Microsoft.Extensions.DependencyInjection;

namespace WalletService.Core.Kafka.Topics;

public static class KafkaTopics
{
    public static string ProcessEcoPoolWithOutTopic { get; set; } = "ecoPool-out";
    public static string ProcessEcoPoolWithInTopic { get; set; } = "ecoPool-in";
    public static string ProcessEcoPoolPaymentTopic { get; set; } = "process-payment";
    public static string ProcessModelFourTopic { get; set; } = "process-model-four";

    public static void RegisterKafkaTopics(this IServiceCollection services, string env)
    {
        var envLower = env.ToLower();
        ProcessEcoPoolWithOutTopic += "-" + envLower;
        ProcessEcoPoolWithInTopic  += "-" + envLower;
        ProcessEcoPoolPaymentTopic += "-" + envLower;
    }
}
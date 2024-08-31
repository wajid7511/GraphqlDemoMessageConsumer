using GraphQl.Mongo.Database.DALs;
using GraphQlDemo.Shared.Messaging;

namespace MessageConsumer.Processors;


public class EmailProcessor(CustomerDAL customerDAL, ILogger<EmailProcessor>? logger = null) : BaseProcessor
{
    private readonly CustomerDAL _customerDAL = customerDAL ?? throw new ArgumentNullException(nameof(customerDAL));
    private readonly ILogger<EmailProcessor>? _logger = logger;
    public override bool CanProcess(MessageType messageType)
    {
        return messageType == MessageType.Order;
    }

    public override async ValueTask<bool> ProcessAsync(MessageDto messageDto)
    {
        Guid orderId = Guid.Parse(messageDto.ReferenceId);
        var dbGetResult = await _customerDAL.GetCustomerOrderByIdAsync(orderId);
        if (dbGetResult.IsError || !dbGetResult.IsSuccess || dbGetResult.Data == null)
        {
            _logger?.LogError("No order found with id {0}", messageDto.ReferenceId);
            return false;
        }
        var dbUpdateResult = await _customerDAL.UpdateCustomerOrder(orderId, dbGetResult.Data);
        if (dbUpdateResult.IsError || !dbUpdateResult.IsSuccess)
        {
            _logger?.LogError(dbUpdateResult.Exception, "Unable to update customer order {0}", dbGetResult.Data.Id.ToString());
        }
        return await base.ProcessAsync(messageDto);
    }
}

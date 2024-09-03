using GraphQl.Mongo.Database.DALs;
using GraphQl.Mongo.Database.Models;
using GraphQlDemo.Shared.Enums;
using GraphQlDemo.Shared.Messaging;

namespace MessageConsumer.Processors;

public class EmailProcessor(CustomerOrderDAL customerOrderDAL, ILogger<EmailProcessor>? logger = null) : BaseProcessor
{
    private readonly CustomerOrderDAL _customerOrderDAL = customerOrderDAL ?? throw new ArgumentNullException(nameof(customerOrderDAL));
    private readonly ILogger<EmailProcessor>? _logger = logger;
    public override bool CanProcess(MessageType messageType)
    {
        return messageType == MessageType.Order;
    }

    public override async ValueTask<bool> ProcessAsync(MessageDto messageDto)
    {
        Guid orderId = Guid.Parse(messageDto.ReferenceId);
        var dbGetResult = await _customerOrderDAL.GetCustomerOrderByIdAsync(orderId);
        if (dbGetResult.IsError || !dbGetResult.IsSuccess || dbGetResult.Data == null)
        {
            _logger?.LogError(dbGetResult.Exception, "No order found with id {0}", messageDto.ReferenceId);
            return false;
        }
        var customerOrder = dbGetResult.Data;
        Dictionary<string, object> updateProperties = new()
        {
            { nameof(CustomerOrder.OrderStatusId), OrderStatusEnum.Processing }
        };

        var dbUpdateResult = await _customerOrderDAL.UpdateCustomerOrder(orderId, updateProperties);
        if (dbUpdateResult.IsError || !dbUpdateResult.IsSuccess)
        {
            _logger?.LogError(dbUpdateResult.Exception, "Unable to update customer order {0}", dbGetResult.Data.Id.ToString());
            return false;
        }
        return await base.ProcessAsync(messageDto);
    }
}

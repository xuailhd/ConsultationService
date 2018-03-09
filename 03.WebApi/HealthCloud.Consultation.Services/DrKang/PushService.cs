using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCloud.Consultation.Services.DrKang
{
    public class MsgPushService
    {
        public bool Push(string orderId,string FromAccountId,string Content,string ToAlias)
        {
            Command.msgPushCommand command = new Command.msgPushCommand(
                OrderID: orderId,
                FromAccountId:FromAccountId,
                ToAlias:ToAlias,
                Content:Content);

            return command.Execute();
        }
    }
}

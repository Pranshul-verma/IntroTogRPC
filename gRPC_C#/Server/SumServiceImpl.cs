using Grpc.Core;
using Sum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sum.Sumservice;

namespace Server
{
    public class SumServiceImpl : SumserviceBase
    {
        public override Task<SumResponce> SUM(SumRequest request, ServerCallContext context)
        {
            var sum = request.Sum.Variable1 + request.Sum.Variable2;
            return Task.FromResult(new SumResponce {Result = sum });
        }
    }
}

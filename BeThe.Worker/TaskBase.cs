//
// 모든작업의 기본 클래스
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeThe.Worker
{
    internal abstract class TaskBase
    {
        // 작업시작
        public abstract void Run();

        // 작업중단
        public abstract void Stop();
    }
}

[Домой](https://pavlenkodr.github.io/XamarinStudents/)

# Lesson 6 multithreading

## Thread

`Thread` представляет собой физический, системный поток выполнения(инкапсулирует его)

Fields:
* **`CurrentThread`** - возвращает ссылку на выполняемый поток
* **`IsAlive`** - работает ли поток
* **`IsBackground`** - фоновый ли поток
* **`Name`** - имя потока
* **`Priority`** - приоритет
  * `Lowest`
  * `BelowNormal`
  * `Normal`
  * `AboveNormal`
  * `Highest`
* **`ThreadState`** - состояние потока 
  * `Aborted` - поток остановлен, но не завершен
  * `AbortRequested`: вызван метод Abort, но остановка потока еще не произошла
  * `Background`: поток выполняется в фоновом режиме
  * `Running`: поток запущен и работает (не приостановлен)
  * `Stopped`: поток завершен
  * `StopRequested`: поток получил запрос на остановку
  * `Suspended`: поток приостановлен
  * `SuspendRequested`: поток получил запрос на приостановку
  * `Unstarted`: поток еще не был запущен
  * `WaitSleepJoin`: поток заблокирован в результате действия методов Sleep или Join

```cs
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    static class ExampleThreading
    {
        private static async Task SafeRun(string methodName, string threadName, Func<Task> callback)
        {
            Console.WriteLine($">>> {threadName} {methodName} Hello");
            try
            {
                await callback();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{threadName} {methodName} failed\n{e.Message}");
            }
            Console.WriteLine($"<<< {threadName} {methodName} Bye");
        }

        private static async void MathRunAsync(string threadName)
        {
            await SafeRun("MathRunAsync", threadName, () =>
            {
                double result = 1.0;
                for (int i = 0; i < 100000; ++i)
                {
                    result = result / Math.Sin((i + 1.0) % 180.0) * (i + 1.0) % 1000.0;
                }
                return Task.CompletedTask;
            });
        }

        private static async Task GetAsync(string threadName, string url)
        {
            await SafeRun("GetAsync", threadName, async () =>
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                Thread.Sleep(2000);
            });
        }

        public static async void Run()
        {
            var url1 = "https://www.youtube.com/watch?v=MnrJzXM7a6o";
            var url2 = "https://www.youtube.com/watch?v=jdoksITir94";

            ThreadStart threadStart = async () =>
            {
                await GetAsync("SideThread", url1);
            };
            threadStart += async () =>
            {
                await GetAsync("SideThread", url2);
            };
            threadStart += () =>
            {
                MathRunAsync("SideThread");
            };
            var thread = new Thread(threadStart);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();

            var thread1 = new Thread(async () =>
            {
                await GetAsync("SideThread2", url1);
                await GetAsync("SideThread2", url2);
                MathRunAsync("SideThread2");
            });
            thread1.Priority = ThreadPriority.Lowest;
            thread1.Start();

            await GetAsync("MainThread", url1);
            await GetAsync("MainThread", url2);
            MathRunAsync("MainThread");

            while (thread.ThreadState != ThreadState.Stopped && thread1.ThreadState != ThreadState.Stopped) { }
            Console.WriteLine("Run() done.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExampleThreading.Run();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

Если нам нужно запустить тред с параметрами, то для этого можно использовать `ParameterizedThreadStart`, но это плохое решение, так как нужно приводить типы.

```cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    static class ExampleThreading
    {
        private static async Task SafeRun(string methodName, Func<Task> callback)
        {
            var thread = Thread.CurrentThread;
            Console.WriteLine($">>> {thread.Name} {methodName} Hello");
            try
            {
                await callback();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{thread.Name} {methodName} failed\n{e.Message}");
            }
            Console.WriteLine($"<<< {thread.Name} {methodName} Bye");
        }

        class Point
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        private static async void RunAsync(object args)
        {
            await SafeRun("MathRunAsync", () =>
            {
                var map = new SortedDictionary<string, int>();
                for (int i = (args as Point).x; i < (args as Point).y; ++i)
                {
                    map.Add(i.ToString(), i);
                }
                for (int i = (args as Point).x; i < (args as Point).y; ++i)
                {
                    var tmp = map[i.ToString()];
                }
                Console.WriteLine($"Result {map.Count}");
                return Task.CompletedTask;
            });
        }

        public static void Run()
        {
            var thread = new Thread(new ParameterizedThreadStart(RunAsync))
            {
                Name = "SideThread"
            };
            thread.Start(new Point() { x = 0, y = 1000000 });

            var thread1 = new Thread(() =>
            {
                RunAsync(new Point() { x = 0, y = 100000 });
            })
            {
                Name = "SideThread1"
            };
            thread1.Start();

            RunAsync(new Point() { x = 0, y = 10000 });

            while (thread.ThreadState != ThreadState.Stopped && thread1.ThreadState != ThreadState.Stopped) { }
            Console.WriteLine("Run() done.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExampleThreading.Run();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

Если потоки используют разделяемые ресурсы, то можно использовать ключевое слово `lock`. В примере мы будем синхронизировать переменную `list`. Внутри это чудо состоит из мониторов (`System.Threading.Monitor`)

```cs
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp2
{
    static class ExampleThreading
    {
        private static void SafeRun(string methodName, Action callback)
        {
            var thread = Thread.CurrentThread;
            Console.WriteLine($">>> {thread.Name} {methodName} Hello");
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{thread.Name} {methodName} failed\n{e.Message}");
            }
            Console.WriteLine($"<<< {thread.Name} {methodName} Bye");
        }

        static List<int> list = new List<int>();

        private static void SafeInsert(int value)
        {
            SafeRun("SafeInsert", () =>
            {
                lock (list)
                {
                    Thread.Sleep(2000);
                    for (int i = 0; i < value; ++i)
                    {
                        list.Add(i);
                    }
                }
            });
        }

        static void customLock(object lockObj, Action callback)
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(lockObj, ref lockWasTaken);
                callback();
            }
            finally
            {
                if (lockWasTaken)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        private static void SafeRead()
        {
            SafeRun("SafeRead", () =>
            {
            customLock(list, () =>
                {
                    Console.WriteLine($"Readed {list.Count}");
                });
            });
        }

        public static void Run()
        {
            var thread = new Thread(() =>
            {
                SafeInsert(10000);
            })
            {
                Name = "SideThread"
            };
            thread.Start();

            var thread1 = new Thread(() =>
            {
                SafeRead();
            })
            {
                Name = "SideThread1"
            };
            thread1.Start();

            while (thread.ThreadState != ThreadState.Stopped && thread1.ThreadState != ThreadState.Stopped) { }
            Console.WriteLine("Run() done.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExampleThreading.Run();
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

Если поток T выполняется в кодовом блоке lock, и ему требуется доступ к ресурсу R, который временно недоступен, то можно освободить поток T с помощью Monitor.Pulse и встать на ожидание ресурса R с помощью Monitor.Wait.

```cs
using System;
using System.Threading;

namespace ConsoleApp2
{
    class Program
    {
        private static object locker = new object();

        static public void Enter()
        {
            lock (locker)
            {
                Console.Write("Enter ");
                Monitor.Pulse(locker);
                Monitor.Wait(locker);
                Thread.Sleep(500);
            }
        }

        static public void Exit()
        {
            lock (locker)
            {
                Console.WriteLine("Exit");
                Monitor.Pulse(locker);
                Monitor.Wait(locker);
            }
        }

        static void Run()
        {
            if (Thread.CurrentThread.Name == "Enter")
            {
                for (int i = 0; i < 5; i++)
                {
                    Enter();
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Exit();
                }
            }
        }

        static void Main()
        {
            var thread1 = new Thread(Run)
            {
                Name = "Enter"
            };
            thread1.Start();
            var thread2 = new Thread(Run)
            {
                Name = "Exit"
            };
            thread2.Start();
            thread1.Join();
            thread2.Join();

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

`Mutex` `WaitOne()` - залочить и `ReleaseMutex()` - разлочить

`Semaphore` аналогично

```cs
static Semaphore sem = new Semaphore(3, 3);
Thread myThread;
int count = 3;
    
public Reader(int i)
{
    myThread = new Thread(Read);
    myThread.Name = $"Читатель {i.ToString()}";
    myThread.Start();
}

public void Read()
{
    while (count > 0)
    {
        sem.WaitOne();
        Console.WriteLine($"{Thread.CurrentThread.Name} Enter");
        Console.WriteLine($"{Thread.CurrentThread.Name} Read");
        Thread.Sleep(1000);
        Console.WriteLine($"{Thread.CurrentThread.Name} Leave");
        sem.Release();
        count--;
        Thread.Sleep(1000);
    }
}
```

## Task Parallel Library

`Task` является абстракцией, представляющей асинхронную операцию.

Исполнением задач управляет планировщик задач, который работает с пулом потоков. Это, например, означает, что несколько задач могут разделять один и тот же поток.

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Start");
            Task task = new Task(() =>
            {
                Console.WriteLine(">");
                for (int i = 0; i < 10; ++i)
                {
                    Thread.Sleep(500);
                    Console.WriteLine("!");
                }
            });
            task.Start();

            for (int i = 0; i < 60; i++)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

`Task.CurrentId` возвращает исполняемую в настоящий момент задачу или же пустое значение, если вызывающий код не является задачей

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo()
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            Task task1 = new Task(Foo);
            Task task2 = new Task(Foo);
            task1.Start();
            Thread.Sleep(250);
            task2.Start();

            for (int i = 0; i < 60; i++)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

С помощью метода `Task.Wait()` можно дождаться окончания выполнения задачи.

Если последовательность завершения задач не важна, то можно дожидаться их окончания с помощью `Task.WaitAll(params Task[] tasks)`

Если нужно дожидаться окончания потоков в другом месте, не блокируя основной поток, то можно вызвать `Task.WhenAll(params Task[] tasks)`, который в свою очередь породит еще один `Task`, завершение которого будет означать окончание переданных в него тасков

Если необходимо дождаться завершения хотя бы одной задачи, то можно воспользоваться `Task.WaitAny(params Task[] tasks)`

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo()
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            Task task1 = new Task(Foo);
            Task task2 = new Task(Foo);
            task1.Start();
            task2.Start();

            Task.WaitAll(task1, task2);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

В программах, которые используют много задач, имеет смысл использовать команду `Task.Dispose()`. Она сообщает gc, что ресурсы, которые использовались в таске, можно уже сейчас освободить.

Из задач можно возвращать значения

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static long Foo()
        {
            var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
        }

        static void Main()
        {
            var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            Console.WriteLine("Start");
            var task1 = new Task<long>(Foo);
            var task2 = new Task<long>(Foo);

            task1.Start();
            task2.Start();

            Task.WaitAll(task1, task2);

            Console.WriteLine($"Task1 time: {task1.Result}");
            Console.WriteLine($"Task2 time: {task2.Result}");
            Console.WriteLine($"Sum time: {DateTimeOffset.Now.ToUnixTimeMilliseconds() - start}");

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

## TaskFactory

Приведенные ранее примеры программ были составлены не так эффективно, как следовало бы, поскольку задачу можно создать и сразу же начать ее исполнение, вызвав метод `StartNew()`, определенный в классе `TaskFactory`. В классе `TaskFactory` предоставляются различные методы, упрощающие создание задач и управление ими.

*Читаем `Task.Factory.StartNew()`, пишем `Task.Run()`*

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo()
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            Task task1 = Task.Factory.StartNew(Foo);
            Task task2 = Task.Factory.StartNew(Foo);

            Task.WaitAll(task1, task2);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

Одной из новаторских и очень удобных особенностей библиотеки TPL является возможность создавать продолжение задачи. Продолжение — это одна задача, которая автоматически начинается после завершения другой задачи. Создать продолжение можно, в частности, с помощью метода `ContinueWith()`, определенного в классе `Task`.

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo()
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            Task task1 = Task.Factory.StartNew(Foo);
            var task2 = task1.ContinueWith((Task task) => {
                Console.WriteLine($"@ Continued task #{task.Id}");
                task.Dispose();
                Foo();
            });

            Task.WaitAll(task1, task2);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

## Cancel Task

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo(CancellationToken cancelTok)
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                if (cancelTok.IsCancellationRequested)
                {
                    Console.WriteLine($"! Task #{Task.CurrentId} canceled");
                    break;
                }
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            var cancellationTokenSource = new CancellationTokenSource();
            var task1 = Task.Factory.StartNew(() =>
            {
                Foo(cancellationTokenSource.Token);
            });
            var task2 = Task.Factory.StartNew(() =>
            {
                Foo(cancellationTokenSource.Token);
            });

            Thread.Sleep(2000);

            cancellationTokenSource.Cancel();

            Task.WaitAll(task1, task2);

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```

## Parallel

Одним из главных классов в TPL является `System.Threading.Tasks.Parallel`. Этот класс поддерживает набор методов, которые позволяют выполнять итерации по коллекции данных (точнее, по объектам, реализующим `IEnumerable<T>`) в параллельном режиме. Этот класс поддерживает два статических метода — `Parallel.For()` и `Parallel.ForEach()`, для каждого из которых определены многочисленные перегруженные версии.

Вызов метода `Break()` формирует запрос на как можно более раннее прекращение параллельно выполняемого цикла, что может произойти через несколько шагов цикла после вызова метода `Break()`. Но все шаги цикла до вызова метода `Break()` все же выполняются. Следует, также иметь в виду, что отдельные части цикла могут и не выполняться параллельно. Так, если выполнено 10 шагов цикла, то это еще не означает, что все эти 10 шагов представляют 10 первых значений переменной управления циклом.

```cs
static long Foo()
{
    var start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    Console.WriteLine($"> Current task #{Task.CurrentId}");
    Parallel.For(0, 10, (i, state) =>
    {
        Thread.Sleep(500);
        Console.WriteLine($"! Current task #{Task.CurrentId} For iteration #{i}");
        state.Break();
    });
    Parallel.ForEach(new int[] { 1, 2, 3, 4 }, (i) =>
    {
        Thread.Sleep(500);
        Console.WriteLine($"! Current task #{Task.CurrentId} ForEach value #{i}");
    });
    return DateTimeOffset.Now.ToUnixTimeMilliseconds() - start;
}
```

Метод `Invoke()`, определенный в классе `Parallel`, позволяет выполнять один или несколько методов, указываемых в виде его аргументов. Ключевое отличие от `Task.Factory.StartNew` состоит в том, что вызов `Parallel.Invoke()` - блокирующий

```cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Foo()
        {
            Console.WriteLine($"> Current task #{Task.CurrentId}");
            for (int i = 0; i < 10; ++i)
            {
                Thread.Sleep(500);
                Console.WriteLine($"! Current task #{Task.CurrentId}");
            }
        }

        static void Main()
        {
            Console.WriteLine("Start");
            Parallel.Invoke(Foo, Foo);
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
```
## Необходимые ссылки

1. [Потоки и файлы](https://professorweb.ru/my/csharp/thread_and_files/1/thread_index.php)
2. [Многопоточность](https://metanit.com/sharp/tutorial/11.1.php)
3. [Параллельное программирование и библиотека TPL](https://metanit.com/sharp/tutorial/12.1.php)
4. [Task.Run vs Task.Factory.StartNew](https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/)

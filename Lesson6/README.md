# Lesson 6 multithreading

Thread
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


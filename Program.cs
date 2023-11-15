namespace MJU23v_D15_inl_todolist
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
            Todo.ReadListFromFile();
            Todo.PrintHelp();
            string command;
            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command, "aktivera"))
                {
                    if (MyIO.NumArguments(command) != 1)
                    {
                        Console.WriteLine("Användning: aktivera /nummer/");
                    }
                    else
                    {
                        string arg = MyIO.Argument(command, 0);
                        Todo.TrySetStatus(arg, Todo.TodoItem.Active);
                    }
                }
                else if (MyIO.Equals(command, "klar"))
                {
                    if (MyIO.NumArguments(command) != 1)
                    {
                        Console.WriteLine("Användning: klar /nummer/");
                    }
                    else
                    {
                        string arg = MyIO.Argument(command, 0);
                        Todo.TrySetStatus(arg, Todo.TodoItem.Ready);
                    }
                }
                else if (MyIO.Equals(command, "vänta"))
                {
                    if (MyIO.NumArguments(command) != 1)
                    {
                        Console.WriteLine("Användning: vänta /nummer/");
                    }
                    else
                    {
                        string arg = MyIO.Argument(command, 0);
                        Todo.TrySetStatus(arg, Todo.TodoItem.Waiting);
                    }
                }
                else if (MyIO.Equals(command, "lista"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: true);
                    else
                        Todo.PrintTodoList(verbose: false);
                }
                else if (MyIO.Equals(command, "sluta"))
                {
                    Console.WriteLine("Hej då!");
                    break;
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: {command}");
                }
            }
            while (true);
        }
    }
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();
        public class TodoItem
        {
            // Constants
            public const int Active = 1;
            public const int Waiting = 2;
            public const int Ready = 3;
            // TODO: make an enum of the above

            // Attribute
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem(int priority, string task)
            {
                this.status = Active;
                this.priority = priority;
                this.task = task;
                this.taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public string ToString(bool verbose = false)
            {
                string res = $"|{StatusString(),-12}|{priority,-6}|{task,-20}|";
                if (verbose)
                    res += $"{taskDescription,-40}|";
                return res;
            }
            public void Print(bool verbose = false)
            {
                Console.WriteLine(ToString());
            }
            public bool SetActive()
            {
                if (status == Waiting)
                {
                    status = Active;
                    return true;
                }
                return false;
            }
            public bool SetReady()
            {
                if (status == Active)
                {
                    status = Ready;
                    return true;
                }
                return false;
            }
            public bool SetWaiting()
            {
                if (status == Active)
                {
                    status = Waiting;
                    return true;
                }
                return false;
            }
            public bool SetStatus(int newStatus)
            {
                switch(newStatus)
                {
                    case Active: return SetActive();
                    case Waiting: return SetWaiting();
                    case Ready: return SetReady();
                    default: return false;
                }
            }
            public string StatusString()
            {
                switch (status)
                {
                    case Active: return "aktiv";
                    case Waiting: return "väntande";
                    case Ready: return "avklarad";
                    default: return "(felaktig)";
                }
            }
        }
        public static void ReadListFromFile()
        {
            string dir = @"..\..\..\";
            string todoFileName = $"{dir}todo.lis";
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
            int numRead = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
        }
        public static void TrySetStatus(string arg, int status)
        {
            try
            {
                int i = Int32.Parse(arg);
                try
                {
                    if (list[i].SetStatus(status)) 
                        switch(status)
                        {
                            case Todo.TodoItem.Active: Console.WriteLine("=> aktiverades!"); break;
                            case Todo.TodoItem.Ready: Console.WriteLine("=> sattes klar!"); break;
                            case Todo.TodoItem.Waiting: Console.WriteLine("=> sattes väntande!"); break;
                            default: Console.WriteLine("Fel: ogilitg status"); break;
                        }
                    else
                        Console.WriteLine("Fel: misslyckades på grund av nuvarande status");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Uppgiften fanns inte!");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Felaktigt nummer!");
            }
        }
        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|# |status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|--|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }
        public static void PrintTodoList(bool verbose = false)
        {
            PrintHead(verbose);
            int i = 0;
            foreach (TodoItem item in list)
            {
                Console.WriteLine($"|{i++,-2}{item.ToString(verbose)}");
            }
            PrintFoot(verbose);
        }
        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("hjälp           skriv ut denna hjälp");
            Console.WriteLine("aktivera /num/  aktivera uppgift (om väntande)");
            Console.WriteLine("klar /num/      sätt uppgift klar (om aktiv)");
            Console.WriteLine("lista           lista att-göra-listan");
            Console.WriteLine("lista allt      lista att-göra-listan utförligt");
            Console.WriteLine("vänta /num/     sätt uppgift väntande (om aktiv)");
            Console.WriteLine("sluta           avsluta programmet");
        }
    }
    public class MyIO
    {
        static public string Argument(string rawCommand, int num)
        {
            if (NumArguments(rawCommand) < num)
            {
                throw new ArgumentException($"Too few arguments to get #{num}!");
            }
            string command = rawCommand.Trim();
            if (command == "") throw new ArgumentException($"Too few arguments to get #{num}!");
            string[] cwords = command.Split(' ');
            return cwords[num + 1];
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;
                if (cwords[1] == expected) return true;
            }
            return false;
        }
        static public int NumArguments(string rawCommand)
        {
            string command = rawCommand.Trim();
            if (command == "") return 0;
            string[] cwords = command.Split(' ');
            return cwords.Length - 1;
        }
        static public string ReadCommand(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}

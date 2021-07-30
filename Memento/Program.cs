using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memento
{
    class Originator 
    {
        private string _state;
        public Originator(string state)
        {
            _state = state;
            Console.WriteLine("My initial state is : " + _state);
        }
        public void DoSomething() 
        {
            Console.WriteLine("Originator : I am doing something important");
            _state = this.GenerateRandomString(30);
        }
        public string GenerateRandomString(int lenght) 
        {
            string allowedSymbols = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";
            while (lenght>0)
            {
                result += allowedSymbols[new Random().Next(0, allowedSymbols.Length)];
                Thread.Sleep(12);
                lenght--;
            }
            return result;
        }
        public IMemento Save() 
        {
            return new TextMemento(_state);
        }
        public void Restore(IMemento memento)
        {
            if (!(memento is TextMemento))
            {
                throw new Exception("Unknown memento class" + memento.ToString());
            }
            _state = memento.GetStates();
        }
    }
    interface IMemento
    {
        string GetName();
        string GetStates();
        DateTime GetDate();
    }

    class TextMemento : IMemento
    {
        private string _state;
        private DateTime _date;
        public TextMemento(string state)
        {
            _state = state;
            _date = DateTime.Now;
        }
        public DateTime GetDate()
        {
            return _date;
        }

        public string GetName()
        {
            return $"{_date} / {_state.Substring(0, 9)}";
        }

        public string GetStates()
        {
            return _state;
        }
    }
    class CareTaker
    {
        private List<IMemento> _mementos = new List<IMemento>();
        private Originator _originator = null;
        public CareTaker(Originator originator)
        {
            _originator = originator;
        }
        public void BackUp() 
        {
            Console.WriteLine("\nCareTaker Saving Originator state . . . ");
            _mementos.Add(_originator.Save());
        }
        public void Undo()
        {
            if (_mementos.Count==0)
            {
                return;
            }
            var memento = _mementos.Last();
            _mementos.Remove(memento);
            Console.WriteLine("\nCareTaker : Restoring state" + memento.GetName());
            try
            {
                _originator.Restore(memento);
            }
            catch (Exception)
            {
                Undo();
            }
        }
        public void ShowHistory() 
        {
            Console.WriteLine("\nCareTaker : Here\'s the list of mementos");
            foreach (var item in _mementos)
            {
                Console.WriteLine(item.GetName());
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Originator originator = new Originator("Super duper super puper super");
            CareTaker careTaker = new CareTaker(originator);
            careTaker.BackUp();
            originator.DoSomething();
            careTaker.BackUp();
            originator.DoSomething();
            careTaker.BackUp();
            originator.DoSomething();
            Console.WriteLine();
            careTaker.ShowHistory();
            Console.WriteLine("\nClient : Now , let\'s rollback!\n");
            careTaker.Undo();
            Console.WriteLine();
            careTaker.Undo();
            Console.WriteLine();
            careTaker.Undo();
            careTaker.ShowHistory();
        }
    }
}

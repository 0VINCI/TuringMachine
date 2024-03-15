using System;
using System.Collections.Generic;
using System.Text;

namespace LM3
{
    // Klasa definiująca pojedyncze przejście w maszynie Turinga
    public class Transition
    {
        public string NewState { get; } // Nowy stan po przejściu
        public char WriteSymbol { get; } // Symbol do zapisania na taśmie
        public char MoveDirection { get; } // Kierunek ruchu głowicy ('R' dla prawo, '-' dla zatrzymania)

        // Konstruktor inicjalizujący przejście z danymi parametrami
        public Transition(string newState, char writeSymbol, char moveDirection)
        {
            NewState = newState;
            WriteSymbol = writeSymbol;
            MoveDirection = moveDirection;
        }
    }
    // Klasa reprezentująca maszynę Turinga
    public class TuringMachine
    {
        private readonly Dictionary<(string, char), Transition> _transitions = new()
        {
            { ("q0", '0'), new Transition("q1", '!', 'R') },
            { ("q0", '1'), new Transition("q2", '!', 'R') },
            { ("q0", '!'), new Transition("q4", '!', '-') }, // Stan końcowy
            { ("q1", '!'), new Transition("q3", '0', '-') }, // Stan końcowy
            { ("q2", '!'), new Transition("q3", '1', '-') }, // Stan końcowy
            // Dodatkowe przejścia dla stanu q3 - po zwiększeniu liczby nie wykonujemy już żadnych operacji
            { ("q3", '0'), new Transition("q3", '0', '-') }, // Nie wykonujemy już żadnych operacji, więc pozostajemy w q3
            { ("q3", '1'), new Transition("q3", '1', '-') }, // Nie wykonujemy już żadnych operacji, więc pozostajemy w q3
            { ("q3", '!'), new Transition("q3", '!', '-') }, // Nie wykonujemy już żadnych operacji, więc pozostajemy w q3
            { ("q4", '0'), new Transition("q4", '0', '-') }, // Maszyna pozostaje w q4, niezależnie od przeczytanego symbolu
            { ("q4", '1'), new Transition("q4", '1', '-') }, // Maszyna pozostaje w q4, niezależnie od przeczytanego symbolu
            { ("q4", '!'), new Transition("q4", '!', '-') }, // Maszyna pozostaje w q4, niezależnie od przeczytanego symbolu

        };


        private readonly StringBuilder _tape = new();
        private string _currentState = "q0";
        private readonly List<string> _statesHistory = new();

        // Metoda ładująca ciąg znaków na taśmę maszyny Turinga
        public void LoadTape(string input)
        {
            _tape.Append(input);
            _statesHistory.Add(_currentState);
        }

        public void Run()
        {
            int headPosition = 0; // Pozycja głowicy na taśmie
            
            // Pętla działa, dopóki głowica nie wyjdzie poza długość taśmy
            while (headPosition < _tape.Length)
            {
                char currentSymbol = _tape[headPosition]; // Aktualny symbol pod głowicą
                var transitionKey = (_currentState, currentSymbol); // Klucz do wyszukania przejścia

                // Sprawdzenie, czy istnieje przejście dla bieżącego stanu i symbolu
                if (_transitions.TryGetValue(transitionKey, out Transition transition))
                {
                    // Wykonanie przejścia
                    _tape[headPosition] = transition.WriteSymbol; // Zapisujemy symbol na taśmie
                    _currentState = transition.NewState; // Aktualizujemy stan
                    _statesHistory.Add(_currentState); // Dodajemy stan do historii
                    
                    // wyswietlamy informacje o przejściu
                    Console.WriteLine($"Stan: {_currentState}, Symbol: {transition.WriteSymbol}, Ruch: {transition.MoveDirection}"); 
                    
                    // Ruch glowicy w prawo
                    if (transition.MoveDirection == 'R') headPosition++;
                    else if (transition.MoveDirection == '-') break; // Zatrzymujemy, jeśli kierunek to '-'
                }
                else
                {                
                    // Gdy nie ma zdefiniowanego przejścia, zatrzymujemy maszynę i wyswietlamy komunikat
                    Console.WriteLine("Brak przejścia dla symbolu: " + currentSymbol + " w stanie: " + _currentState);
                    break;
                }
            }
            
            // wyswietlamy końcowy stan maszyny i pełną ścieżkę stanów
            Console.WriteLine("Końcowy stan MT: " + _currentState);
            Console.WriteLine("Ścieżka stanów: " + string.Join(" -> ", _statesHistory));
        }

        public void PrintTape()    // Metoda wypisująca aktualny stan taśmy
        {
            Console.WriteLine("Stan taśmy: " + _tape.ToString());
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            TuringMachine tm = new TuringMachine(); // Tworzymy nowy obiekt maszyny Turinga
            
            // wyswietlamy komunikat i oczekujemy na wejście użytkownika
            Console.WriteLine("Wprowadź jednocyfrową liczbę binarną zakończoną znakiem '!': ");
            string input = Console.ReadLine();

            // Sprawdzamy poprawność wejścia
            if (input.Length == 2 && (input[0] == '0' || input[0] == '1') && input[1] == '!')
            {
                tm.LoadTape(input); // Ładujemy wejście na taśmę
                tm.Run(); // Uruchamiamy symulację
                tm.PrintTape(); // wyswietlamy stan taśmy po przetworzeniu
            }
            else
            {
                // Jeśli wejście jest nieprawidłowe, wyswietlamy komunikat o błędzie
                Console.WriteLine(
                    "Nieprawidłowe wejście. Upewnij się, że wprowadzasz jednocyfrową liczbę binarną zakończoną znakiem '!'.");
            }
        }
    }
}

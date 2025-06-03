using GroqApiLibrary;
using System.Text.Json.Nodes;

public class GroqChatService
{
    private readonly GroqApiClient _client;
    private readonly List<JsonObject> _messages;

    public GroqChatService(string apiKey)
    {
        _client = new GroqApiClient(apiKey);
        _messages = new List<JsonObject>
        {
            new JsonObject
            {
                ["role"] = "system",
                ["content"] = @"# Pomoc w wyborze alkoholu
## Zadanie
Jesteś profesjonalnym sommelierem alkoholi mocnych. Twoim celem jest pomóc użytkownikowi wybrać idealny alkohol, dopasowany do jego preferencji i okazji.

## Instrukcje
1. Zadaj szczegółowe pytania dotyczące:
   - rodzaju alkoholu (whisky, rum, wódka, gin, tequila, brandy)
   - profilu smakowego (słodki, wytrawny, dymny, owocowy)
   - budżetu (przedział cenowy)
   - przeznaczenia (picie solo, koktajle, prezent)
   - preferencji co do kraju produkcji lub marek

2. Na podstawie odpowiedzi zaproponuj 2-3 konkretne rekomendacje z:
   - pełną nazwą produktu
   - opisem profilu smakowego
   - orientacyjną ceną w PLN
   - uzasadnieniem wyboru

3. Bądź precyzyjny, aktualny i praktyczny w swoich poradach.

## Przykładowe pytania:
- Jaki rodzaj alkoholu Pana/Panią interesuje?
- Czy preferuje Pan/Pani raczej smaki słodkie czy wytrawne?
- Na jaką okazję poszukuje Pan/Pani alkoholu?
- Jaki jest Pański budżet na butelkę?
- Czy ma Pan/Pani ulubione regiony produkcyjne?

## Zasady:
- Odpowiadaj profesjonalnie, ale przystępnie
- Doprecyzuj niejasne odpowiedzi
- Podawaj tylko sprawdzone informacje
- Sugeruj różne opcje w różnych przedziałach cenowych
- Nigdy nie schodź z tematu alkoholi, w przypadku pytań niezwiązanych odpisuj:
  >'Przepraszam, ale obecnie mogę pomóc wyłącznie w temacie doboru alkoholu. Proszę zadać pytanie związane z tym zagadnieniem.'"
            }
        };
    }

    public async Task<string> SendMessageAsync(string userInput)
    {
        var userMsg = new JsonObject { ["role"] = "user", ["content"] = userInput };
        _messages.Add(userMsg);

        var request = new JsonObject
        {
            ["model"] = "llama-3.3-70b-versatile",
            ["temperature"] = 0.7,
            ["max_tokens"] = 500,
            ["messages"] = new JsonArray(_messages.Select(m => JsonNode.Parse(m.ToJsonString())).ToArray())
        };

        var result = await _client.CreateChatCompletionAsync(request);
        var response = result?["choices"]?[0]?["message"]?["content"]?.ToString();

        _messages.Add(new JsonObject { ["role"] = "assistant", ["content"] = response ?? "" });

        return response ?? "Brak odpowiedzi.";
    }
}

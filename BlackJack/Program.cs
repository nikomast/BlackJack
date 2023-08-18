// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

Game peli_testi = new Game();
peli_testi.play();

public class Card
{
    public int arvo;
    public int maa;
    public Card(int a, int m)
    {
        arvo = a;
        maa = m;
    }

    public void print_card()
    {
        Console.WriteLine("Arvo: " + arvo + " " + "Maa: " + maa);
    }
}

public class Deck
{
    int lkm = 6;
    public List<Card> deck = new List<Card>();

    public Deck()
    {
        for (int i = 0; i < lkm; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 13; k++)
                {
                    deck.Add(new Card(k, j));
                }
            }
        }

    }

    public void suffle()
    {
        Random rnd = new Random();
        int min = 0;
        int max = deck.Count();

        for (int i = 0; i < deck.Count() * 2; i++)
        {

            int temp = rnd.Next(min, max);
            int random = rnd.Next(min, max);
            deck[temp] = deck[random];

        }

    }

}

public class Player
{
    public string name;
    public float bank;
    public bool stay;
    public List<Card> hand = new List<Card>();
    public List<Card> all = new List<Card>();
    public int count = 0;

    public Player(string n, float b)
    {
        name = n;
        bank = b;
    }

    public void show_hand()
    {
        Console.WriteLine("Players hand: ");
        for (int i = 0; i < hand.Count(); i++)
        {
            hand[i].print_card();
        }
    }

    public int bet_amount(int minimum)
    {

        for (int i = 0; i < all.Count; i++)
        {
            if (all[i].arvo > 1 && all[i].arvo < 6)
            {
                count++;
            }
            else if (all[i].arvo > 9)
            {
                count--;
            }

        }
        bank -= minimum;
        return minimum;
    }

    public int CalculateHandTotal(List<Card> hand)
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in hand)
        {
            if (card.arvo == 1)
            {
                aceCount++;
                total += 11;
            }
            else if (card.arvo > 10)
            {
                total += 10;
            }
            else
            {
                total += card.arvo;
            }
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }

    public int dec(Deck cards)
    {
        int total = CalculateHandTotal(hand);
        int decksRemaining = cards.deck.Count / 52;
        int trueCount = count / decksRemaining;  // Assume `count` is available here

        if (total <= 8)
        {
            return 1;
        }
        else if (total == 9)
        {
            return trueCount >= 3 ? 1 : 0;
        }
        else if (total == 10)
        {
            return trueCount >= 4 ? 1 : 0;
        }
        else if (total >= 12 && total <= 16)
        {
            return trueCount >= 0 ? 0 : 1;
        }
        else if (total >= 17)
        {
            return 0;
        }
        else if (hand.Any(c => c.arvo == 1) && total >= 13 && total <= 14)
        {
            return trueCount >= 5 ? 1 : 0;
        }
        else if (hand.Any(c => c.arvo == 1) && total >= 15 && total <= 16)
        {
            return trueCount >= 4 ? 1 : 0;
        }
        else if (hand.Any(c => c.arvo == 1) && total == 17)
        {
            return trueCount >= 3 ? 1 : 0;
        }
        else if (hand.Any(c => c.arvo == 1) && total == 18)
        {
            if (trueCount < 3)
            {
                return 1;
            }
            else if (trueCount >= 4)
            {
                return 1;
            }
            return 0;
        }
        else if (hand.Any(c => c.arvo == 1) && (total == 19 || total == 20))
        {
            return 0;
        }
        // Add further conditions for pairs, if required

        return 0; // Default action
    }
}


public class Game
{

    public Deck cards = new Deck();
    Player pelaaja = new Player("Nimi", 1000);
    public List<Card> hand = new List<Card>();
    public int min_bet = 10;
    public int bet = 0;
    public List<int> wieners = new List<int>();

    public void play()
    {
        start();
        while (cards.deck.Count > 78)
        {
            start();
        };
        Console.WriteLine(pelaaja.bank);
    }
    public void print_deck()
    {
        for (int i = 0; i < cards.deck.Count(); i++)
        {
            cards.deck[i].print_card();
        }
    }
    public void print_dealers_hand()
    {
        Console.WriteLine("Dealers hand: ", hand.Count());
        for (int i = 0; i < hand.Count(); i++)
        {
            hand[i].print_card();
        }
    }
    public int get_hand_total(List<Card> temp)
    {
        return temp.Sum(card => card.arvo);
    }
    public void more(List<Card> temp)
    {
        temp.Add(cards.deck[0]);
        pelaaja.all.Add(cards.deck[0]);
        cards.deck.RemoveAt(0);
    }
    public void start()
    {
        cards.suffle();
        bet = pelaaja.bet_amount(min_bet);
        //Panoksen asettamiseen tarvitaan logiikka
        for (int i = 0; i < 2; i++)
        {

            more(pelaaja.hand);
            more(hand);
        }

        deal();

    }
    public void deal()
    {

        if (pelaaja.dec(cards) == 1)
        {
            more(pelaaja.hand);
        }
        else
        {
            while (get_hand_total(hand) < 17)
            {
                more(hand);
            }
        }
        end();
    }
    public void end()
    {
        int w = wiener();
        if (w == 1)
        {
            pelaaja.bank += bet * 2;
        }
        else if (w == 0)
        {
            pelaaja.bank -= bet;
        }
        else
        {
            pelaaja.bank += bet / 2;
        }
        wieners.Add(wiener());
        clear();
    }
    public int wiener()
    {
        int playerTotal = get_hand_total(pelaaja.hand);
        int dealerTotal = get_hand_total(hand);

        // Check for busts
        if (playerTotal > 21)
        {
            return 0;
        }
        else if (dealerTotal > 21)
        {
            return 1;
        }

        // Check for blackjacks
        if (playerTotal == 21 && pelaaja.hand.Count == 2)
        {
            return 1; // Player has a blackjack
        }
        else if (dealerTotal == 21 && hand.Count == 2)
        {
            return 0; // Dealer has a blackjack
        }

        // Compare hand totals
        if (playerTotal > dealerTotal)
        {
            return 1;
        }
        else if (dealerTotal > playerTotal)
        {
            return 0;
        }
        else
        {
            return 99; // Tie
        }
    }
    public void clear()
    {
        pelaaja.hand.Clear();
        hand.Clear();
    }
}




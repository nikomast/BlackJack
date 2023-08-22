// See https://aka.ms/new-console-template for more information
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
int i = 0;
List<int> tulokset = new List<int>();
while (i < 100)
{
    Game peli_testi = new Game();
    peli_testi.play();
    i++;
}

//Määritellään kortti
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

//Määritellään korttipakka
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
                {   if (k > 10)
                    {
                        deck.Add(new Card(10, j));
                    }
                    else {
                        deck.Add(new Card(k, j));
                    }
                    
                }
            }
        }

    }

    //Korttipakan toiminto, joka sekoittaa kortit
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

//Määritellään pelaaja
public class Player
{
    public string name;
    public float bank;
    public List<Card> hand = new List<Card>();
    //Tässä on kaikki kortit, jotka pelaaja on nähnyt
    public List<Card> all = new List<Card>();
    //Korttien laskun säilytys
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

    //Pelaajan logiikkaa, joka on ainakin panoksen asettamisen kannalta täyttä paskaa
    public int get_count(int decks_remaining)
    {

        foreach (var card in all)
        {
            if (card.arvo > 2 && card.arvo < 7)
            {
                count++;
            }
            else if (card.arvo > 6 && card.arvo < 10)
            {
          
            }
            else if (card.arvo > 9)
            {
                count--;
            }
        }

        return count / decks_remaining;
    }

    //Tässä tehdään päätös otetaanko lisää kortteja vai ei
    public int stay(Card dealers_hand)
    {
        int total = hand.Sum(card => card.arvo);
        if (total > 16 && dealers_hand.arvo > 6) 
        {
            return 0;
        }
        else if (dealers_hand.arvo < 7 && total > 14) 
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }
}


//Määritellään peli, joka on tässä tapauksessa black jack
public class Game
{

    public Deck cards = new Deck();
    Player pelaaja = new Player("Nimi", 1000);
    public List<Card> hand = new List<Card>();
    public int min_bet = 10;
    public int bet = 0;
    public List<int> wieners = new List<int>();

    //Pelin logiikka
    public void play()
    {
        while (cards.deck.Count > 78)
        {
            start();
        };
        //Paljonko jäi käteen?
        Console.WriteLine(pelaaja.bank);
        
    }

    public void start()
    {
        cards.suffle();

        int temp = pelaaja.get_count(cards.deck.Count() / 52);
        if (temp > 0)
        {
            bet = min_bet + temp;
            pelaaja.bank -= bet;
        }
        else
        {
            bet = min_bet;
            pelaaja.bank -= bet;
        }

        more(hand);
        for (int i = 0; i < 2; i++)
        {
            more(pelaaja.hand);
        }

        deal();

    }

    public void more(List<Card> temp)
    {
        temp.Add(cards.deck[0]);
        pelaaja.all.Add(cards.deck[0]);
        cards.deck.RemoveAt(0);
    }

    public void deal()
    {

        if (pelaaja.stay(hand[0]) == 1)
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

        if (playerTotal > 21)
        {
            return 0;
        }
        else if (dealerTotal > 21)
        {
            return 1;
        }

        if (playerTotal == 21 && pelaaja.hand.Count == 2)
        {
            return 1;
        }
        else if (dealerTotal == 21 && hand.Count == 2)
        {
            return 0;
        }

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
            return 99;
        }
    }

    public int get_hand_total(List<Card> temp)
    {
        return temp.Sum(card => card.arvo);
    }

    public void clear()
    {
        pelaaja.hand.Clear();
        hand.Clear();
    }

    public void print_dealers_hand()
    {
        Console.WriteLine("Dealers hand: ", hand.Count());
        for (int i = 0; i < hand.Count(); i++)
        {
            hand[i].print_card();
        }
    }

}




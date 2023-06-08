using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "HangmanDictionary", menuName = "Hangman/Dictionary")]
public class HangmanDictionary : ScriptableObject
{
    // Contains the theme and words for the hangman system
    // Wanting to create a better method of storing these.


    private Dictionary<string, string[]> wordsByTheme { get; set; } = new Dictionary<string, string[]>
    {
        {"Demons", new string[] {"Bael", "Agares", "Vassago", "Samigina", "Marbas", "Valefar", "Aamon", "Barbatos", "Paimon", "Buer",
        "Gusion", "Sitri", "Beleth", "Leraje", "Eligos", "Zepar", "Botis", "Bathin", "Sallos", "Purson",
        "Marax", "Ipos", "Aim", "Naberius", "Glasya-Labolas", "Bifrons", "Vual", "Haures", "Crocell", "Alloces",
        "Vapula", "Forneus", "Foras", "Amdusias", "Vual", "Decarabia", "Andras", "Haures", "Andrealphus", "Cimeies",
        "Vassago", "Phenex", "Halpas", "Sabnock", "Forneus", "Balam", "Alloces", "Vual", "Vapula", "Gaap",
        "Belial", "Gremory", "Purson", "Aamon", "Vassago", "Ronove", "Berith", "Aamon", "Leraje", "Forneus",
         "Gusion", "Vapula", "Asmoday", "Gaap", "Focalor", "Bune", "Ronove", "Berith"}}, // all known demons
        {"Gnosticism", new string[] {"Archon", "Demiurge", "Pleroma", "Samael", "Sophia", "Aeon", "Emanation", "Hylics"}}, // all known gnostic terms & concepts
        {"Fiction", new string[] {"Hogwarts", "Gandalf", "Sauron", "Mordor", "Darth Vader", "Hobbit", "Middle-earth", "Narnia"}}, // all known fictional things
        {"Angels", new string[] {"Gabriel", "Michael", "Raphael","Uriel","Azrael","Metatron","Sandalphon","Raziel","Anael","Zadkiel","Jophiel","Samael","Haniel", "Remiel","Barachiel","Chamuel","Sariel","Sachiel","Ariel","Jerahmeel","Selaphiel","Amitiel","Zerachiel","Ramiel","Tzaphkiel", "Tzadkiel","Kamael","Zophiel","Barbiel", "Yerachmiel", "Rachmiel","Zehanpuryu","Abaddon", "Araqiel","Azaziel", "Ithuriel","Jegudiel","Jehoel", "Penemue","Phanuel", "Raguel", "Suriel","Turiel", "Zagzagel", "Zuphlas", "Zuriel"}}, // all known angels
        {"Cosmic", new string[] {"Nebula", "Quasar", "Supernova", "Black hole", "Dark matter", "Red giant", "Cosmic microwave background", "Gamma ray burst", "non-infinite universe", "Penrose Diagrams", "Penrose-Carter diagrams", "Minkowski Spacetime", "Hawking radiation", "Neutrino",}},
        {"Matter", new string[] {"Gas", "Solid", "Liquid", "Plasma", "Bose-Einstein Condensate", "Time Crystal", "Neutronium", "Quark-Gluon Plasma", "Fermionic condensate", "Degenerate Matter", "Quantum Hall", "Rydberg Matter", "Rydberg polaron", "Strange matter", "Superfluid", "Supersolid", "Photonic molecules", "QCD matter", "Lattice QCD", "Quark-gluon plasma", "Color-glass condensate", "Supercritical fluid", "Colloid", "Glass", "Crystal", "Liquid crystal", "Quantum spin fluid", "Exotic Matter", "Programmable matter", "Dark matter", "Antimatter", "Magnetically ordered", "Antiferromagnet", "Ferrimagnet", "Ferromagnet", "String-net liquid", "Superglass", "Quasiparticle", "Island of Stability", "proton", "neutron", "Hydrogen", "Helium", "Lithium", "Beryllium", "Boron", "Carbon", "Nitrogen", "Oxygen", "Fluorine", "Neon", "Sodium", "Magnesium", "Aluminum", "Silicon", "Phosphorus", "Sulfur", "Chlorine", "Argon", "Potassium", "Calcium", "Scandium", "Titanium", "Vanadium", "Chromium", "Manganese", "Iron", "Cobalt", "Nickel", "Copper", "Zinc", "Gallium", "Germanium", "Arsenic", "Selenium", "Bromine", "Krypton", "Rubidium", "Strontium", "Yttrium", "Zirconium", "Niobium", "Molybdenum", "Technetium", "Ruthenium", "Rhodium", "Palladium", "Silver", "Cadmium", "Indium", "Tin", "Antimony", "Tellurium", "Iodine", "Xenon", "Cesium", "Barium", "Lanthanum", "Cerium", "Praseodymium", "Neodymium", "Promethium", "Samarium", "Europium", "Gadolinium", "Terbium", "Dysprosium", "Holmium", "Erbium", "Thulium", "Ytterbium", "Lutetium", "Hafnium", "Tantalum", "Tungsten", "Rhenium", "Osmium", "Iridium", "Platinum", "Gold", "Mercury", "Thallium", "Lead", "Bismuth", "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Thorium", "Protactinium", "Uranium", "Neptunium", "Plutonium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Fermium", "Mendelevium", "Nobelium", "Lawrencium", "Rutherfordium", "Dubnium", "Seaborgium", "Bohrium", "Hassium", "Meitnerium", "Darmstadtium", "Roentgenium", "Copernicium", "Nihonium", "Flerovium", "Moscovium", "Livermorium", "Tennessine", "Oganesson", "subatomic particle" }}, // add transitions, quantities and concepts
       {"Geometry", new string[] {"deSitter Space", "anti-deSitter Space", "compactification", "Kruskal-Szekeres coordinates", "Eddington-Finkelstein coordinates", "conformal", "Euclidean", "Escher's Circle Limit IV", "Poincare disk", "geodesics", "tessellation", "hyperbolic space", "polygon"  } }

    };

    internal string[] GetAllWords()
    {
        return wordsByTheme.Values.SelectMany(x => x).ToArray();
    }

    internal string[] GetWordsByTheme(string theme)
    {
        if (wordsByTheme.TryGetValue(theme, out string[] words))
        {
            return words;
        }
        else
        {
            // handle case where theme is not found in dictionary
            throw new KeyNotFoundException($"Theme '{theme}' not found in dictionary.");
        }
    }

    // When finished, add a alchemy & et cetera dictionary value to wordsByTheme


}

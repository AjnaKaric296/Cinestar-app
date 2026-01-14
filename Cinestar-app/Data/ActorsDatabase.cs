using Cinestar_app.Models;
using System.Collections.Generic;

namespace Cinestar_app.Data
{
    public static class ActorsDatabase
    {
        public static List<Actor> AllActors { get; } = new List<Actor>
        {
            new Actor { Name = "Robert Downey Jr.", Photo = "Actors/robert.jpg" },
            new Actor { Name = "Scarlett Johansson", Photo = "Actors/scarlett.jfif" },
            new Actor { Name = "Chris Evans", Photo = "Actors/chris.jfif" },
            new Actor { Name = "Chris Hemsworth", Photo = "Actors/hemsworth.jfif" },
            new Actor { Name = "Mark Ruffalo", Photo = "Actors/mark.jfif" },
            new Actor { Name = "Jeremy Renner", Photo = "Actors/jeremy.jfif" },
            new Actor { Name = "Tom Holland", Photo = "Actors/tom.jfif" },
            new Actor { Name = "Benedict Cumberbatch", Photo = "Actors/benedict.jfif" },
            new Actor { Name = "Elizabeth Olsen", Photo = "Actors/elizabeth.jfif" },
            new Actor { Name = "Paul Rudd", Photo = "Actors/paul.jfif" },
            new Actor { Name = "Chris Pratt", Photo = "Actors/pratt.jfif" },
            new Actor { Name = "Zoe Saldana", Photo = "Actors/zoe.jfif" },
            new Actor { Name = "Vin Diesel", Photo = "Actors/vin.jfif" },
            new Actor { Name = "Bradley Cooper", Photo = "Actors/bradley.jfif" },
            new Actor { Name = "Dave Bautista", Photo = "Actors/dave.jfif" },
            new Actor { Name = "Karen Gillan", Photo = "Actors/karen.jfif" },
            new Actor { Name = "Pom Klementieff", Photo = "Actors/pom.jfif" },
            new Actor { Name = "Sean Gunn", Photo = "Actors/sean.jfif" },
            new Actor { Name = "Michael Rooker", Photo = "Actors/michael.jfif" },
            new Actor { Name = "Chris Sullivan", Photo = "Actors/sullivan.jfif" },
            new Actor { Name = "Anthony Mackie", Photo = "Actors/anthony.jfif" },
            new Actor { Name = "Sebastian Stan", Photo = "Actors/sebastian.jfif" },
            new Actor { Name = "Don Cheadle", Photo = "Actors/don.jfif" },
            new Actor { Name = "Benedict Wong", Photo = "Actors/wong.jfif" },
            new Actor { Name = "Chadwick Boseman", Photo = "Actors/chadwick.jfif" },
            new Actor { Name = "Letitia Wright", Photo = "Actors/letitia.jfif" },
            new Actor { Name = "Danai Gurira", Photo = "Actors/danai.jfif" },
            new Actor { Name = "Winston Duke", Photo = "Actors/winston.jfif" },
            new Actor { Name = "Martin Freeman", Photo = "Actors/martin.jfif" },
            new Actor { Name = "Taika Waititi", Photo = "Actors/taika.jfif" }
        };
    }
}

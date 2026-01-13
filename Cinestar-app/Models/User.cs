using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace Cinestar_app.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Ime { get; set; }
    public string Prezime { get; set; }
    public string Email { get; set; }

    public string Lozinka { get; set; }  // za jednostavnost, cuvamo plaintext (u pravoj aplikaciji hashirati!)
    public int LoyaltyPoints { get; set; }
}

using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Data;

namespace project;

public class AppDbContext : DbContext
{
    public MySqlConnection connection;
    private string server;
    private string database;
    private string uid;
    private string password;

    public AppDbContext(DbContextOptions<AppDbContext> options)
: base(options)
    {
        Initialize();
    }

    // Initialize values
    private void Initialize()
    {
        server = "localhost";
        database = "exchangedb";
        uid = "root1";
        password = "Pisyakota1";
        string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                           database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

        connection = new MySqlConnection(connectionString);
    }

    // Open connection to the database
    public bool OpenConnection()
    {
        try
        {
            // Attempt to open the database connection
            connection.Open();

            // Return true if the connection is successfully opened
            return true;
        }
        catch (MySqlException ex)
        {
            switch (ex.Number)
            {
                case 0:
                    // Unable to connect to the server
                    Console.WriteLine("Cannot connect to the server. Contact the administrator.");
                    break;

                case 1045:
                    // Invalid username/password
                    Console.WriteLine("Invalid username/password. Please try again.");
                    break;
            }

            return false;
        }
    }

    // Close the database connection
    public bool CloseConnection()
    {
        try
        {
            // Attempt to close the database connection
            connection.Close();

            // Return true if the connection is successfully closed
            return true;
        }
        catch (MySqlException ex)
        {
            // Display the exception message if an error occurs during closing
            Console.WriteLine(ex.Message);

            // Return false if an exception occurs during closing
            return false;
        }
    }
}

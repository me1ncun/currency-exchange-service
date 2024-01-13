using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using MySql.Data.MySqlClient;
using project;
using System.Reflection.PortableExecutable;

namespace Exchange.Database.Repositories;

public class CurrenciesRepository
{
    private AppDbContext dbContext;

    public CurrenciesRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public bool Create(string name, string code, string sign)
    {
        try
        {
        string query = $"INSERT INTO currencies (code, fullname, sign) VALUES(@code, @name, @sign)";

        if (dbContext.OpenConnection() == true)
        {
            // create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);

            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("code", code);
            cmd.Parameters.AddWithValue("sign", sign);

            // Execute command
            cmd.ExecuteNonQuery();

            // close connection
            dbContext.CloseConnection();
        }
        return true;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public List<Currency> Read()
    {
        try
        {
        string query = "SELECT * FROM currencies";

        List<Currency> currencies = new List<Currency>();

        if (dbContext.OpenConnection() == true)
        {
            // Execute the MySQL query
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            // Read and populate account data from the result set
            while (dataReader.Read())
            {
                Currency currency = new Currency
                {
                    ID = Convert.ToInt32(dataReader["id"]),
                    Code = dataReader["code"].ToString(),
                    Name = dataReader["fullName"].ToString(),
                    Sign = dataReader["sign"].ToString()
                };

                currencies.Add(currency);
            }

            // Close data reader and connection
            dataReader.Close();
            dbContext.CloseConnection();

            // Return the list of accounts
            return currencies;
        }
        else
        {
            // Return an empty list if the connection couldn't be opened
            return currencies;
        }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public void Update(Currency currency)
    {
        try
        {
        // MySQL query to update account data based on the account name
        string query =
            $"UPDATE currencies SET code=@code, fullName = @name, sign = @sign WHERE id=@id";

        // Open the database connection
        if (dbContext.OpenConnection() == true)
        {
            // Execute the update query
            MySqlCommand cmd = new MySqlCommand();

            cmd.Parameters.AddWithValue("id", currency.ID);
            cmd.Parameters.AddWithValue("code", currency.Code);
            cmd.Parameters.AddWithValue("name", currency.Name);
            cmd.Parameters.AddWithValue("sign", currency.Sign);

            cmd.CommandText = query;
            cmd.Connection = dbContext.connection;
            cmd.ExecuteNonQuery();

            // Close the database connection
            dbContext.CloseConnection();
        }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public Currency ReadByID(int id)
    {
        try
        {
        string query = $"SELECT * FROM currencies WHERE ID = @id";

        Currency currency = null;

        if (dbContext.OpenConnection() == true)
        {
            // Execute the MySQL query
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);

            cmd.Parameters.AddWithValue("id", id);

            MySqlDataReader dataReader = cmd.ExecuteReader();
            // Read and populate account data from the result set
            while (dataReader.Read())
            {
                currency = new Currency
                {
                    ID = Convert.ToInt32(dataReader["id"]),
                    Code = dataReader["code"].ToString(),
                    Name = dataReader["fullName"].ToString(),
                    Sign = dataReader["sign"].ToString()
                };
            }

            // Close data reader and connection
            dataReader.Close();
            dbContext.CloseConnection();

            // Return the list of accounts
            return currency;
        }
        else
        {
            // Return an empty list if the connection couldn't be opened
            return currency;
        }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }
}



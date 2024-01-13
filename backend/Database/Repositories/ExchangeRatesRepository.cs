using Exchange.Database.Services;
using Exchange.Mapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using project;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Exchange.Database.Repositories;

public class ExchangeRatesRepository
{
    private AppDbContext dbContext;
    private CurrenciesService currenciesService;
    private CurrenciesRepository currenciesRepository;
    public ExchangeRatesRepository(AppDbContext dbContext)
    {
        this.dbContext = dbContext;

        currenciesRepository = new CurrenciesRepository(dbContext);
        currenciesService = new CurrenciesService(currenciesRepository);
    }
    public bool Create(string baseCurrencyCode, string targetCurrencyCode, decimal rate)
    {
        try
        {
        var firstCurrencyID = currenciesService.GetCurrency(baseCurrencyCode).ID;
        var secondCurrencyID = currenciesService.GetCurrency(targetCurrencyCode).ID;

        string query = "INSERT INTO exchangerates (BaseCurrencyId, TargetCurrencyId, Rate) VALUES(@firstCurrencyID, @secondCurrencyID, @rate);";

        if (dbContext.OpenConnection() == true)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, dbContext.connection))
            {
                // Add parameters
                cmd.Parameters.AddWithValue("@firstCurrencyID", firstCurrencyID);
                cmd.Parameters.AddWithValue("@secondCurrencyID", secondCurrencyID);
                cmd.Parameters.AddWithValue("@rate", rate);

                // Execute command
                cmd.ExecuteNonQuery();
            }

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

    public List<ExchangeRateDTO> Read()
    {
        try
        {
        // MySQL query to select all data from the 'exchangerates' table
        string query = "SELECT * FROM exchangerates";
        List<ExchangeRate> exchangeRateList = new List<ExchangeRate>();

        List<ExchangeRateDTO> exchangeRateDTOList = new List<ExchangeRateDTO>();

        if (dbContext.OpenConnection() == true)
        {
            // Execute the MySQL query
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            // Read and populate account data from the result set
            while (dataReader.Read())
            {
                ExchangeRate exchangeRate = new ExchangeRate
                {
                    ID = int.Parse(dataReader["ID"].ToString()),
                    BaseCurrencyID = int.Parse(dataReader["BaseCurrencyId"].ToString()),
                    TargetCurrencyID = int.Parse(dataReader["TargetCurrencyId"].ToString()),
                    Rate = decimal.Parse(dataReader["Rate"].ToString()),
                };

                exchangeRateList.Add(exchangeRate);
            }
            dbContext.CloseConnection();

            foreach (var exchangeRate in exchangeRateList)
            {
                ExchangeRateDTO exch = new ExchangeRateDTO();

                ExchangeMapper mapper = new ExchangeMapper(currenciesRepository);
                exch = mapper.GetTo(exchangeRate);

                exchangeRateDTOList.Add(exch);
            }
            dataReader.Close();
            dbContext.CloseConnection();
            return exchangeRateDTOList;
        }
        else
        {
            return exchangeRateDTOList;
        }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public ExchangeRateDTO GetExchangeRateByCode(string code)
    {
        try
        {
        string query = "SELECT * FROM exchangerates";

        string firstCurrencyCode = code.Substring(0, 3);
        string secondCurrencyCode = code.Substring(3, 3);

        Currency firstCurrency = currenciesService.GetCurrency(firstCurrencyCode);
        Currency secondCurrency = currenciesService.GetCurrency(secondCurrencyCode);

        ExchangeRateDTO exchangeRateDTO = new ExchangeRateDTO();

        if (dbContext.OpenConnection() == true)
        {
            // Execute the MySQL query
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            // Read and populate account data from the result set
            while (dataReader.Read())
            {
                if (firstCurrency.ID == int.Parse(dataReader["BaseCurrencyID"].ToString()) && secondCurrency.ID == int.Parse(dataReader["TargetCurrencyID"].ToString()))
                {
                    exchangeRateDTO = new ExchangeRateDTO
                    {
                        ID = int.Parse(dataReader["ID"].ToString()),
                        BaseCurrency = firstCurrency,
                        TargetCurrency = secondCurrency,
                        Rate = decimal.Parse(dataReader["Rate"].ToString()),
                    };
                }
            }

            // Close data reader and connection
            dataReader.Close();
            dbContext.CloseConnection();

            // Return the list of accounts
            return exchangeRateDTO;
        }
        else
        {
            // Return an empty list if the connection couldn't be opened
            return exchangeRateDTO;
        }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public ExchangeRateDTO UpdateExchangeRate(string code, decimal rate)
    {
        try
        {
        Currency firstCurrency = currenciesService.GetCurrency(code.Substring(0, 3));
        Currency secondCurrency = currenciesService.GetCurrency(code.Substring(3, 3));

        string query = "UPDATE exchangerates SET Rate = @Rate WHERE BaseCurrencyId = @BaseCurrencyId AND TargetCurrencyId = @TargetCurrencyId;";

        ExchangeRateDTO exchangeRateDTO = GetExchangeRateByCode(code);

        if (dbContext.OpenConnection())
        {
            // Execute the MySQL query with parameters
            MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);
            cmd.Parameters.AddWithValue("@Rate", rate);
            cmd.Parameters.AddWithValue("@BaseCurrencyId", firstCurrency.ID);
            cmd.Parameters.AddWithValue("@TargetCurrencyId", secondCurrency.ID);

            try
            {
                // ExecuteNonQuery is used for UPDATE, INSERT, DELETE, etc.
                int affectedRows = cmd.ExecuteNonQuery();

                // Close the connection
                dbContext.CloseConnection();

                // Check if the update was successful (at least one row affected)
                if (affectedRows > 0)
                {
                    // Return the updated ExchangeRateDTO
                    return GetExchangeRateByCode(code);
                }
            }
            catch (MySqlException ex)
            {
                // Handle the exception, log, or rethrow as needed
                Console.WriteLine($"MySQL Exception: {ex.Message}");
            }
        }

        // Return the original ExchangeRateDTO if the update was not successful or the connection couldn't be opened
        return exchangeRateDTO;
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

    public ExchangeRateDTO GetExchangeRateByTwoCodes(string baseCurrencyCode, string targetCurrencyCode, decimal rate)
    {
        try
        {
            string query = "SELECT * FROM exchangerates WHERE BaseCurrencyId = @BaseCurrencyId AND TargetCurrencyId = @TargetCurrencyId;";

            Currency baseCurrency = currenciesService.GetCurrency(baseCurrencyCode);
            Currency targetCurrency = currenciesService.GetCurrency(targetCurrencyCode);

            ExchangeRateDTO exchangeRateDTO = new ExchangeRateDTO();

            if (dbContext.OpenConnection() == true)
            {
                // Execute the MySQL query
                MySqlCommand cmd = new MySqlCommand(query, dbContext.connection);

                cmd.Parameters.AddWithValue("@BaseCurrencyId", baseCurrency.ID);
                cmd.Parameters.AddWithValue("@TargetCurrencyId", targetCurrency.ID);

                MySqlDataReader dataReader = cmd.ExecuteReader();

                // Read and populate account data from the result set
                while (dataReader.Read())
                {
                        exchangeRateDTO = new ExchangeRateDTO
                        {
                            ID = int.Parse(dataReader["ID"].ToString()),
                            BaseCurrency = baseCurrency,
                            TargetCurrency = targetCurrency,
                            Rate = decimal.Parse(dataReader["Rate"].ToString()),
                        };
                }

                // Close data reader and connection
                dataReader.Close();
                dbContext.CloseConnection();

                // Return the list of accounts
                return exchangeRateDTO;
            }
            else
            {
                // Return an empty list if the connection couldn't be opened
                return exchangeRateDTO;
            }
        }
        catch (SqlException ex)
        {
            throw new ApplicationException(ex.Message);
        }
    }

}
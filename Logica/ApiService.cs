using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using softvago_API.Logica;
using softvago_API.Models;

public class ApiService
{
    private readonly HttpClient _client;
    private readonly DataQuery _dataQuery;
    private const string ApiUrl = "https://jooble.org/api/";
    private const string ApiKey = "3f5453d5-ce7f-42c0-8bd1-ac74b5e9725b";

    public ApiService()
    {
        _client = new HttpClient();
        _dataQuery = new DataQuery();
    }

    public async Task SearchJobsAsync(JobSearchParameters searchParameters)
    {
        var JobsList = await GetJobsAPIJooble(searchParameters);
        await InsertJobsDB(JobsList);
    }

    private async Task<List<Job>> GetJobsAPIJooble(JobSearchParameters searchParameters)
    {
        List<Job> jobs = new List<Job>();

        var jsonRequest = new
        {
            keywords = searchParameters.Keywords,
            location = searchParameters.Location,
            radius = searchParameters.Radius,
            salary = searchParameters.Salary,
            page = searchParameters.Page,
            searchMode = searchParameters.SearchMode,
            datePublication = searchParameters.DatePublication
        };

        var jsonContent = JsonConvert.SerializeObject(jsonRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _client.PostAsync(ApiUrl + ApiKey, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseBody);
            var jobList = jsonResponse["jobs"].ToObject<dynamic>();

            foreach (var item in jobList)
            {
                var job = new Job
                {
                    id = item.id,
                    title = item.title,
                    enterprise = item.company,
                    urlRedirection = item.link,
                    shortDescription = item.snippet,
                    location = item.location,
                    wage = item.salary != ""
                        ? ConvertSalary(item.salary)
                        : 0.0,
                    idModality = ConvertModality(item.title),
                };
                jobs.Add(job);
            }

            return jobs;
        }
        catch (Exception e)
        {
            // TODO : Log error
            return jobs;
        }
    }

    private double ConvertSalary(dynamic salary)
    {
        salary = salary.ToString();
        salary = salary.Replace("$", "").Trim();
        if (salary.EndsWith("k", StringComparison.OrdinalIgnoreCase))
        {
            salary = salary.Substring(0, salary.Length - 1);
            return Convert.ToDouble(salary) * 1000;
        }

        return Convert.ToDouble(salary);
    }

    private int ConvertModality(dynamic modality)
    {
        string modalityString = modality.ToString();
        int valor = 4;

        var keywords = new Dictionary<string, int>
        {
            { "Onsite ", 1 },
            { "In-office ", 1 },
            { "Remote", 2 },
            { "Hybrid", 3 }
        };

        foreach (var word in keywords)
        {
            if (modalityString.IndexOf(word.Key, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                valor = word.Value;
            }
        }

        return valor;
    }

    private async Task<bool> InsertJobsDB(List<Job> jobs)
    {
        return (await _dataQuery.InsertJobs(jobs) > 0);
    }
}
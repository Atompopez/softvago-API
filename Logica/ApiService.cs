using System.Diagnostics.Metrics;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using softvago_API.Logica;
using softvago_API.Models;

public class ApiService
{
    private readonly HttpClient _client;
    private readonly DataQuery _dataQuery;
    private string ApiUrlJooble;
    private string ApiUrlAdzuna;

    IEnumerable<string> countries = new[]
    {
        "gb", "us", "at", "au", "be",
        "br", "ca", "ch", "de", "es",
        "fr", "in", "it", "mx", "nl",
        "nz", "pl", "sg", "za"
    };


    public ApiService()
    {
        _client = new HttpClient();
        _dataQuery = new DataQuery();
    }

    public async Task SearchJobsAsync(JobSearchParameters searchParameters)
    {
        Task<List<Job>> task1 = GetJobsAPIJooble(searchParameters);
        Task<List<Job>> task2 = GetJobsForAllCountriesAsync(searchParameters);

        var results = await Task.WhenAll(task1, task2);
        //var results = await Task.WhenAll(task2);

        List<Job> jobsFromJooble = results[0];
        List<Job> jobsFromAdzuna = results[1];
        List<Job> allJobs = jobsFromJooble.Concat(jobsFromAdzuna).ToList();
        //List<Job> allJobs = jobsFromAdzuna.ToList();

        await InsertJobsDB(allJobs);
    }

    private async Task<List<Job>> GetJobsForAllCountriesAsync(JobSearchParameters searchParameters)
    {
        var allJobs = new List<Job>();
        int maxConcurrency = 2;
        var semaphore = new SemaphoreSlim(maxConcurrency);

        var tasks = countries.Select(async country =>
        {
            await semaphore.WaitAsync();
            try
            {
                var jobs = await GetJobsAPIAdzuna(searchParameters, country);
                lock (allJobs)
                {
                    allJobs.AddRange(jobs);
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        //allJobs = await GetJobsAPIAdzuna(searchParameters, "gb");
        return allJobs;
    }

    private async Task<List<Job>> GetJobsAPIAdzuna(JobSearchParameters searchParameters, string country)
    {
        string id = "665f7346";
        string key = "1525bff2cee672129bef2fdaa766f4b1";
        ApiUrlAdzuna = $"https://api.adzuna.com/v1/api/jobs/{country}/search/1?app_id={id}&app_key={key}&what={searchParameters.Keywords}";
        List<Job> jobs = new List<Job>();

        try
        {
            var response = await _client.GetAsync(ApiUrlAdzuna);
            response.EnsureSuccessStatusCode();

            using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
            {
                var responseBody = await reader.ReadToEndAsync();

                var jsonResponse = JObject.Parse(responseBody);
                var jobList = jsonResponse["results"].ToObject<dynamic>();

                foreach (var item in jobList)
                {
                    var job = new Job
                    {
                        id = item.id,
                        title = item.title,
                        enterprise = item.company.display_name,
                        urlRedirection = item.redirect_url,
                        shortDescription = item.description,
                        location = item.location.display_name,
                        wage = ConvertSalaryAdzuna(double.Parse(item.salary_max.ToString())),
                        idModality = ConvertModality(item.title, item.location),
                    };
                    jobs.Add(job);
                }
            }
            return jobs;
        }
        catch (Exception e)
        {
            return jobs;
        }
    }

    private async Task<List<Job>> GetJobsAPIJooble(JobSearchParameters searchParameters)
    {
        string ApiKey = "3f5453d5-ce7f-42c0-8bd1-ac74b5e9725b";
        ApiUrlJooble = "https://jooble.org/api/";
        List<Job> jobs = new List<Job>();

        var jsonRequest = new
        {
            keywords = searchParameters.Keywords,
            location = searchParameters.Location
        };

        var jsonContent = JsonConvert.SerializeObject(jsonRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        });
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            var response = await _client.PostAsync(ApiUrlJooble + ApiKey, content);
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
                        ? ConvertSalaryAdzuna(item.salary)
                        : 0.0,
                    idModality = ConvertModality(item.title, item.location),
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

    private double ConvertSalaryAdzuna(double salary)
    {
        return salary * 1.05;
    }

    private double ConvertSalaryJooble(dynamic salary)
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

    private int ConvertModality(dynamic title, dynamic location)
    {
        string titleString = title.ToString();
        string locationString = location.ToString();
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
            if (titleString.IndexOf(word.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                valor = word.Value;

            if (locationString.IndexOf(word.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                valor = word.Value;

            if (valor != 4)
                return valor;
        }
        return valor;
    }

    private async Task<bool> InsertJobsDB(List<Job> jobs)
    {
        return (await _dataQuery.InsertJobs(jobs) > 0);
    }
}
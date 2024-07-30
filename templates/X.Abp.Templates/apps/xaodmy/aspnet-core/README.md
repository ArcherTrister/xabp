# Microservice Solution

This is a startup template to create microservice based solutions.

## Documentation

See [the documentation](https://docs.abp.io/en/commercial/latest/startup-templates/microservice/index) to understand, run and get started with this startup template.



## Tye run

**Tye** is a tool created by Microsoft which makes developing, testing, and deploying microservices and distributed applications easier. Check out [Tye getting started document](https://github.com/dotnet/tye/blob/master/docs/getting_started.md) and install [Tye CLI](https://github.com/dotnet/tye) to your computer   

To run the whole microservice solution, run `run-tye.ps1` PowerShell script which is located in the root directory of your solution. This script automatically creates developer certificates, runs Docker infrastructure applications and starts the web application.

> Beware, you may have to wait a long time as your first run will pull Docker images and run all applications.



### Explore the applications

Once all the applications start, you can open the *Tye Dashboard* by navigating to http://localhost:8000/. The *dashboard* is the UI for *Tye* that displays a list of all of your services. You can see the URLS and logs of the services. 



#### Entry Point Web Application

Each proje type has different port. Start your web application from one the following addresses:

- **Blazor Server**: [https://localhost:44314](https://localhost:44314/)
- **Blazor Web Assembly**: [https://localhost:44307](https://localhost:44307/)
- **MVC**: [https://localhost:44321](https://localhost:44321/)
- **Angular**: [http://localhost:4200](http://localhost:4200/)

##### Default login credentials:

- **Username**: `admin` 
- **Password**: `1q2w3E*` 



#### Public Web Application

Open https://localhost:44335/ URL in your browser to test the public website application.



#### Grafana UI

The solution has been configured to use *Prometheus* and *Grafana*. Open http://localhost:3000/ URL in your browser to run the Grafana UI. Use `admin` as username and `admin` as the password. You can find the `prometheus-net` dashboard at http://localhost:3000/dashboards.
# Tretton37WebScraper

A console app to recursively traverse and download a website and save it to disk. The app finds the count of urls on first level of the website. Then it uses it to calculate progress when downloading the website recursively 

To install and run, just download the source code. The app is composed of two parts:

**Tretton37WebScraper:** a client console app project.

**Tretton37WebScraper.Core:** a library project implementing the core functionality.

The app can be run executing Tretton37WebScraper.exe

## Configuration

By default, the app will download Tretton37 website and save it to C:\temp\Tretton37. 

This is configured on App.config settings file.

Replace BaseUrl with the url you want to download.

Replace BasePath with the path you want to download your website to.

<pre>
<code>
&lt;?xml version="1.0" encoding="utf-8" ?&gt; 
&lt;configuration>
  &lt;appSettings>
    &lt;add key="ApplicationName" value="Tretton37WebScraper"/>
    &lt;add key="BaseUrl" value="https://tretton37.com/"/>
    &lt;add key="BasePath" value="C:\temp\Tretton37"/>
  &lt;/appSettings>
&lt;/configuration>
</code>
</pre>

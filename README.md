# C# Link-Shortner
##An Online Link Shortener Written in C# made to work with NgRok

This is a C# Application that provides a Web interface over NgRok tunneling to create and use shortened links.

## How to Use
1. Run NgRok with the Command "./ngrok http 80" This will create a tunnel for your application to reach the internet
2. Run the C# Program with Admin Privilages, it automaticaly sources the NgRok address and will tell you both the HTTP and HTTPS address of the Site
3. That is it, visit the NgRok Site and you should see a prompt to enter a link, this will then create a shortened link for you when you press the "Get Shortened Link Button"

# Readme

## Docker commands

### Building and deploying on localhost

```
# build docker image
$ docker build -t uconfig .

# run
$ docker run -d -p 8080:80 --name uconfig uconfig

# shutdown
$ docker rm --force uconfig

# full cycle re-deploy
$ docker rm --force uconfig && docker build -t uconfig . && docker run -d -p 8080:80 --name uconfig uconfig
```

## Deploy to Heroku

### Preparing frontend
- in react app, app/services/Config.ts should expose herokuConfig
- run build on react app: npm run build
- copy build/ folder's content under wwwroot/webapp
- make sure that appsettings.json is referncing the correct mysql database


### Deploy Heroku
```
$ heroku login
$ heroku container:login
$ docker build -t uconfig .
$ heroku container:push -a uconfy web # from uconfig/uconfig, where Dockerfile sits
$ heroku container:release -a uconfy web
$ heroku logs --tail -a uconfy
```

## Deploy to AWS

### Preparing frontend
- in react app, app/services/Config.ts should expose awsConfig
- run build on react-uconfy
- content of build folder to be moved into wwwroot/webapp

### Deploy AWS
- login $ aws ecr get-login-password --region eu-west-2 | docker login --username AWS --password-stdin 684109116329.dkr.ecr.eu-west-2.amazonaws.co
- $ docker build -t uconfy-dotnet .
- $ docker tag uconfy-dotnet:latest 684109116329.dkr.ecr.eu-west-2.amazonaws.com/uconfy-dotnet:latest
- $ docker push 684109116329.dkr.ecr.eu-west-2.amazonaws.com/uconfy-dotnet:latest
- login to AWS, navigate to EC2, log in with console
- stop and remove existing container
- $ docker pull 684109116329.dkr.ecr.eu-west-2.amazonaws.com/uconfy-dotnet:latest
- $ docker run -d -p 80:80 --name uconfy10 684109116329.dkr.ecr.eu-west-2.amazonaws.com/uconfy-dotnet

## Configuration for localhost/heroku

- appsettings.json references the correct DB
- app/services/Config.ts should reference devConfig or localDockerConfig
- air sensor device is referencing the correct URL (192.168.1.10:8080)

## TODOs

- go live ("productizing") requirements
  - documentation
  - activity tab
  x rotate api key
  x device card
  x key constraint: should be max 15 length; verify on ui
  - make sure that device logs are XSS safe
  - Add validation: create devices, update device, add config key-value
  x loading indicator on devices screen
  x use rdbms instead of in-memory store
  x rename page title, add favicon
  x auth
    x make all endpoints api key protected
    x introduce JWT token
    x handle incorrect login
    x auto login after page refresh if session is alive
    x check login issue
- features
  - control device: send restart command
  - device-already-read indicator
  - show microcontroller data (cpu, memory, etc)
  - device instance based on MAC address
  - return stale data while editing
  - ask for confirmation before executing btn-danger actions
  - log error from device, hlight in red
  - be able to log all stored configs
- nice to have
  - Install swagger
  - End to end tests for Web API
  - rename all "uconfig" to "uconfy"
  - Console.WriteLine to logger
- security
  x validate email
  - force https
  x jwt token secret to be read from config
  - mysql connection string to be read from environment variables
  x check what happens if jwt validation fails
  - logout: clear jwt from local storage, and invalidate on service
- tech debt
  - remove old log items (eg. retain last 150 always for a device)
  - main site to be on angular
  - web app to be on React

## Use cases

- managing PIN code of immobiliser
- co2 sensor to be accessible in different wifi zones

## Features

- Remote logging
- Remote configuration
- Wifi swap by remote configuration
- Diagnostics and telemetry
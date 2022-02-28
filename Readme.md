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

### Deploy to Heroku

$ heroku login
$ heroku container:login
$ docker build -t uconfig .
$ heroku container:push -a uconfy web # from uconfig/uconfig, where Dockerfile sits
$ heroku container:release -a uconfy web
$ heroku logs --tail -a uconfy

## Configuration for localhost/heroku

- appsettings.json references the correct DB
- js/app.js referencing the correct backend service
- air sensor device is referencing the correct URL (192.168.1.10:8080)

## TODOs

- go live requirements
  - key constraint: should be max 15 length; verify on ui
  - make sure that device logs are XSS safe
  - Add validation: create devices, update device, add config key-value
  x use rdbms instead of in-memory store
  - rename page title, add favicon
  - auth
    x make all endpoints api key protected
    x introduce JWT token
    - handle incorrect login
- features
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
- security
  - jwt token secret to be read from config
  - mysql connection string to be read from environment variables
  - check what happens if jwt validation fails
- tech debt
  - remove old log items (eg. retain last 150 always for a device)

## Use cases

- managing PIN code of immobiliser
- co2 sensor to be accessible in different wifi zones

## Features

- Remote logging
- Remote configuration
- Wifi swap by remote configuration
- Diagnostics and telemetry
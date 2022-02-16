# Readme

## Docker commands

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

## TODOs

- use rdbms instead of in-memory store
- introduce JWT token
- handle incorrect login
- Add validation: create devices, update device, add config key-value
- Install swagger
- End to end tests for Web API
- show microcontroller data (cpu, memory, etc)
- device instance based on MAC address
- device-already-read indicator
- return stale data while editing
- ask for confirmation before executing btn-danger actions
- make all endpoints api key protected

## Use cases

- managing PIN code of immobiliser
- co2 sensor to be accessible in different wifi zones
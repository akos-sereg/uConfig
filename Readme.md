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

Device controller
- use rdbms instead of in-memory store

Login controller
- introduce JWT token
- handle incorrect login

Tech Debt
- Install swagger
- End to end tests for Web API


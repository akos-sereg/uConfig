# Readme

## Docker commands

```
# build docker image
$ docker build -t uconfig .

# run
$ docker run -d -p 8080:80 --name uconfig uconfig

# shutdown
$ docker rm --force uconfig
```

## TODOs

- Install swagger
- End to end tests for Web API
- Use MySQL storage


docker rm --force uconfig && docker build -t uconfig . && docker run -d -p 8080:80 --name uconfig uconfig
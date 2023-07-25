#!/bin/bash
docker service create --replicas 1 --network=testnet --name builder --constraint='node.role==worker' dwjbosman/builder1-distcc
#docker service create --replicas 1 --network=testnet --name builder1-distcc-service --publish published=3632,target=3632 dwjbosman/builder1-distcc

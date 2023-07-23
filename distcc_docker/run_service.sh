docker service create --replicas 1 --name builder1-distcc-service --publish published=3632,target=3632 dwjbosman/builder1-distcc

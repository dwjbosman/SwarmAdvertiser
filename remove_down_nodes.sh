#!/bin/bash

# Get the IDs of all the down nodes
down_nodes=$(docker node ls --format "{{.ID}} {{.Status}}" | grep ' Down' | awk '{print $1}')

# For each down node ID, attempt to remove the node
for node in $down_nodes; do
    echo "Removing $node"
    docker node rm $node --force
done

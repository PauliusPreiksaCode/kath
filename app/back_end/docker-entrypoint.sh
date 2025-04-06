#!/bin/bash
set -e

# Start Redis server
echo "Starting Redis server..."
service redis-server start
status=$?
if [ $status -ne 0 ]; then
    echo "Failed to start Redis: $status"
    exit $status
fi

# Create data directories if they don't exist
mkdir -p src/workspace/fasta src/workspace/revel

# Download FASTA file if it doesn't exist
if [ ! -f "src/workspace/fasta/hg38.fa" ]; then
    echo "Downloading FASTA file..."
    cd src/workspace/fasta
    curl -O https://hgdownload.cse.ucsc.edu/goldenPath/hg38/bigZips/hg38.fa.gz
    gunzip hg38.fa.gz
    cd ../../..
fi

# Download REVEL file if it doesn't exist
if [ ! -f "src/workspace/revel/revel_with_transcript_ids.db" ]; then
    echo "Downloading REVEL file..."
    cd src/workspace/revel
    curl -O https://rothsj06.dmz.hpc.mssm.edu/revel-v1.3_all_chromosomes.zip
    unzip revel-v1.3_all_chromosomes.zip
    cd ../..
    python3 scripts/revel.py workspace/revel/revel_with_transcript_ids workspace/revel/revel_with_transcript_ids.db
    cd ..
fi

# Start the Flask application with gunicorn
echo "Starting Flask application..."
exec gunicorn -c gunicorn_config.py run:app
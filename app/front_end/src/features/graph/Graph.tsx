import { Box, Button, CircularProgress, Grid, Typography, useTheme } from "@mui/material";
import { useGetGraphEntries } from "@/hooks/entry";
import { OrganizationContext } from "@/services/organizationProvider";
import { useContext, useEffect, useState, useMemo, useCallback } from "react";
import { Paths } from "@/types";
import { useNavigate } from "react-router-dom";
import ForceGraph2D from 'react-force-graph-2d';
import ViewEntryCard from "../entry/components/ViewEntryCard";

export interface GraphProps {
    id: string;
    name: string;
    linkedEntries: string[];
};

export default function Graph() {

    const [graphEntries, setGraphEntries] = useState<any>([]);
    const [selectedNode, setSelectedNode] = useState<GraphProps | null>(null);
    const [openViewEntry, setOpenViewEntry] = useState<boolean>(false);
    const organizationContext = useContext(OrganizationContext);
    const Theme = useTheme();
    const navigate = useNavigate();
    const getGraphEntries = useGetGraphEntries(organizationContext.organizationId);

    useEffect(() => {
        setGraphEntries(getGraphEntries.data || []);
    }, [getGraphEntries.data]);

    if(getGraphEntries.isLoading && !getGraphEntries.isFetching) {
        return <CircularProgress/>;
    }

    const graphData = useMemo(() => {
        const nodes = graphEntries.map((entry: GraphProps) => ({
          id: entry.id,
          name: entry.name,
        }));
        
        const links: { source: string; target: string }[] = [];
        graphEntries.forEach((entry: GraphProps) => {
          if (entry.linkedEntries && entry.linkedEntries.length > 0) {
            entry.linkedEntries.forEach(linkedId => {
              links.push({
                source: entry.id,
                target: linkedId
              });
            });
          }
        });
        
        return { nodes, links };
      }, [graphEntries]);

    const handleNodeClick = useCallback((node : GraphProps) => {
        setSelectedNode(node);
        setOpenViewEntry(true);
    }, []);


    return (
        <>
        <Box sx={{
            display: 'flex',
            flexDirection: 'column',
            gap: '1rem',
            padding: '1.2rem',
        }}>
            <Box sx={{
                mt: '0.5rem',
                mb: '0.5rem',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
            }}>
                <Button
                    variant="contained"
                    sx={{
                        width: '10rem',
                        backgroundColor: Theme.palette.primary.main,
                        color: Theme.palette.primary.contrastText,
                        fontSize: '1rem',
                    }}
                    onClick={() => {
                        navigate(Paths.ENTRIES);
                    }}
                >
                    Back
                </Button>
                <Typography sx={{ 
                    fontSize: '2.5rem', 
                    fontWeight: 'bold', 
                    color: Theme.palette.primary.main,
                    flex: 1,
                    textAlign: 'center'
                }}>
                    Relation Graph
                </Typography>
            </Box>
            <Grid container  sx={{ maxHeight: '70vh', width: 'auto', display: 'flex', justifyContent: 'center', alignItems: 'center', position: 'relative', overflow: 'hidden' }}>
                <ForceGraph2D
                    graphData={graphData}
                    nodeLabel={(node: GraphProps) => node.name}
                    nodeColor={() => "#1976d2"}
                    nodeRelSize={10}
                    linkColor={Theme.palette.divider}
                    linkDirectionalArrowLength={4}
                    linkDirectionalArrowRelPos={1}
                    linkDirectionalParticles={2}
                    linkDirectionalParticleSpeed={0.003}
                    onNodeClick={handleNodeClick}
                    enableNodeDrag={true}
                    enableZoomInteraction={true}
                    minZoom={0.1}
                    maxZoom={8}
                    backgroundColor={Theme.palette.background.default}
                    cooldownTicks={0}
                    nodeCanvasObject={(node: any, ctx, globalScale) => {
                        const label = node.name;
                        const fontSize = 14 / globalScale;
                        const nodeR = 5;
                        const isSelected = selectedNode && selectedNode.id === node.id;

                        if (isSelected) {
                            ctx.beginPath();
                            ctx.arc(node.x || 0, node.y || 0, nodeR + 1, 0, 2 * Math.PI);
                            ctx.fillStyle = Theme.palette.primary.main + '33';
                            ctx.fill();
                        }
                
                        ctx.beginPath();
                        ctx.arc(node.x, node.y, 4, 0, 2 * Math.PI, false);
                        ctx.fillStyle = selectedNode && selectedNode.id === node.id ? Theme.palette.primary.main: Theme.palette.secondary.main;
                        ctx.fill();
                        ctx.strokeStyle = "black";
                        ctx.lineWidth = 1;
                        ctx.stroke();

                        ctx.strokeStyle = isSelected ? Theme.palette.primary.dark : Theme.palette.secondary.dark;
                        ctx.lineWidth = 1.5 / globalScale;
                        ctx.stroke();

                        if (globalScale > 1.2 || isSelected) {
                            const textWidth = ctx.measureText(label).width;
                            const bckgDimensions = [textWidth + 2, fontSize + 1].map(n => n + 2 * 0.5);

                        ctx.fillStyle = isSelected ? Theme.palette.primary.light + 'CC' : Theme.palette.background.paper + 'E6';
                        ctx.fillRect((node.x || 0) - bckgDimensions[0] / 2, (node.y || 0) + nodeR + 2, bckgDimensions[0], bckgDimensions[1]);
                
                        ctx.font = `${isSelected ? 'bold ' : ''}${fontSize}px Arial, sans-serif`;
                        ctx.textAlign = 'center';
                        ctx.textBaseline = 'middle';
                        ctx.fillStyle = isSelected ? Theme.palette.primary.contrastText : Theme.palette.text.primary;
                        ctx.fillText(label, node.x || 0, (node.y || 0) + nodeR + 2 + bckgDimensions[1] / 2);
                    }}}
                />
            </Grid>
        </Box>
        {selectedNode && 
        <ViewEntryCard
            open={openViewEntry}
            onClose={() => setOpenViewEntry(false)}
            entryId={selectedNode.id}
        />}
        </>
    );
}
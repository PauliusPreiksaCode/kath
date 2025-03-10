import { useGetEntries } from "@/hooks/entry";
import { OrganizationContext } from "@/services/organizationProvider";
import { Paths } from "@/types";
import { Box, Button, CircularProgress, Grid, Typography, useTheme } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CreateEntryCard from "./components/CreateEntryCard";
import EntryCard from "./components/EntryCard";
import entryUpdates from "@/hooks/entryUpdates";

export interface EntryProps {
    id: string;
    name: string;
    text: string;
    creationDate: string;
    modifyDate: string;
    fileId: string;
    file: {
        id: string;
        name: string;
        extension: number;
        uploadDate: string;
    }
    fullName: string;
    licencedUserId: string;
};

export default function Entry() {

    const [entries, setEntries] = useState<any>([]);
    const [openCreateEntry, setOpenCreateEntry] = useState<boolean>(false);
    const organizationContext = useContext(OrganizationContext);
    const Theme = useTheme();
    const getEntries = useGetEntries(organizationContext.organizationId, organizationContext.groupId);
    const navigate = useNavigate();

    entryUpdates(organizationContext.organizationId);

    useEffect(() => {
        setEntries(getEntries.data || []);
    }, [getEntries.data]);

    if(getEntries.isLoading && !getEntries.isFetching) {
        return <CircularProgress/>;
    }

    return (
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
                        organizationContext.setGroupSessionId('');
                        navigate(Paths.GROUP);
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
                    Entries
                </Typography>
                <Button
                    variant="contained"
                    sx={{
                        width: '10rem',
                        backgroundColor: Theme.palette.primary.main,
                        color: Theme.palette.primary.contrastText,
                        fontSize: '1rem',
                    }}
                    onClick={() => setOpenCreateEntry(true)}
                >
                    Create Entry
                </Button>
            </Box>
            <Grid container spacing={2} sx={{ maxHeight: '70vh', overflowY: 'auto' }}>
                {entries.map((entry: EntryProps) => (
                    <Grid item xs={12} key={entry.id}>
                        <EntryCard {...entry} />
                    </Grid>
                ))}
            </Grid>
            <CreateEntryCard
                open={openCreateEntry}
                onClose={() => setOpenCreateEntry(false)}
            />
        </Box>

    );
};
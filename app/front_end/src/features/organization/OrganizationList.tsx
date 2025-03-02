import { useGetOrganizations } from "@/hooks/organization";
import { Box, Button, CircularProgress, Grid, Typography, useTheme } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import OrganizationCard from "./components/OrganizationCard";
import CreateOrganizationCard from "./components/CreateOrganizationCard";
import { UserContext } from "@/services/authProvider";

export interface Organization {
    id: number;
    name: string;
    description: string;
    creationDate: string;
    groups: object[];
    membersCount: number;
    ownerId: string;
}

export default function Organization() {

    const [organizations, setOrganizations] = useState<any>([]);
    const [openCreateOrganization, setOpenCreateOrganization] = useState<boolean>(false);
    const getOrganizations = useGetOrganizations();
    const Theme = useTheme();
    const userContext = useContext(UserContext);

    useEffect(() => {
        setOrganizations(getOrganizations.data || [])
    }, [getOrganizations.data])

    if(getOrganizations.isLoading || getOrganizations.isFetching) {
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
                <Typography sx={{ 
                    fontSize: '2.5rem', 
                    fontWeight: 'bold', 
                    color: Theme.palette.primary.main,
                    flex: 1,
                    textAlign: 'center'
                }}>
                    Organizations
                </Typography>
                {userContext?.roles?.includes('OrganizationOwner') && 
                    <Button
                        variant="contained"
                        sx={{
                            width: '10rem',
                            backgroundColor: Theme.palette.primary.main,
                            color: Theme.palette.primary.contrastText,
                            fontSize: '1rem',
                        }}
                        onClick={() => setOpenCreateOrganization(true)}
                    >
                        Create Organization
                    </Button>
                }
            </Box>
    
            <Grid container spacing={2} sx={{ maxHeight: '70vh', overflowY: 'auto' }}>
                {organizations.map((organization: Organization) => (
                    <Grid item xs={12} sm={6} md={6} lg={6} key={organization.id}>
                        <OrganizationCard {...organization} />
                    </Grid>
                ))}
            </Grid>
            <CreateOrganizationCard
                open={openCreateOrganization}
                onClose={() => setOpenCreateOrganization(false)}
            />
        </Box>
    );
}
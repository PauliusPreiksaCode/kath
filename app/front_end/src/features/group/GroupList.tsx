import { useGetGroups } from "@/hooks/group";
import { UserContext } from "@/services/authProvider";
import { OrganizationContext } from "@/services/organizationProvider";
import { Paths } from "@/types";
import { Box, Button, CircularProgress, Grid, Typography, useTheme } from "@mui/material";
import { useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import CreateGroupCard from "./components/CreateGroupCard";
import GroupCard from "./components/GroupCard";

export interface GroupProps {
    id: string;
    name: string;
    description: string;
    creationDate: string;
    organizationId: string;
    entriesCount: number;
}


export default function Group() {

    const [groups, setGroups] = useState<any>([]);
    const [openCreateGroup, setOpenCreateGroup] = useState<boolean>(false);
    const userContext = useContext(UserContext);
    const organizationContext = useContext(OrganizationContext);
    const Theme = useTheme();
    const getGroups = useGetGroups(organizationContext.organizationId);
    const navigate = useNavigate();

    useEffect(() => {
        setGroups(getGroups.data || []);
    }, [getGroups.data]);

    if(getGroups.isLoading || getGroups.isFetching) {
        return (
            <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100vh" }}>
              <CircularProgress />
            </Box>
        );
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
                        organizationContext.clearGroupSessionId();
                        organizationContext.clearOrganizationSessionId();
                        navigate(Paths.ORGANIZATION);
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
                    Groups
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
                        onClick={() => setOpenCreateGroup(true)}
                    >
                        Create Group
                    </Button>
                }
            </Box>
            <Grid container spacing={2} sx={{ maxHeight: '70vh', overflowY: 'auto' }}>
                {groups.map((group: GroupProps) => (
                    <Grid item xs={12} sm={12} md={8} lg={6} key={group.id}>
                        <GroupCard {...group} />
                    </Grid>
                ))}
            </Grid>
            <CreateGroupCard
                open={openCreateGroup}
                onClose={() => setOpenCreateGroup(false)}
            />
        </Box>
    );
};
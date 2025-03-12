import { ORGANIZATION_SOCKET_URL } from "@/types/constants/constants";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { useQueryClient } from "@tanstack/react-query";
import { useEffect, useState } from "react";


export default function entryUpdates(organizationId: string) {
    const queryClient = useQueryClient();
    const [connection, setConnection] = useState<any>(null);

    useEffect(() => {
        if (!organizationId) return;

        const newConnection = new HubConnectionBuilder()
            .withUrl(`${ORGANIZATION_SOCKET_URL}/entriesHub`)
            .withAutomaticReconnect()
            .build();

        newConnection.start().then(() => {
            newConnection.invoke("SubscribeToOrganization", organizationId);
        })
        .catch((error: any) => console.error('Connection failed', error));

        newConnection.on("UpdateEntries", (updateOrgId: string) => {
            if (updateOrgId === organizationId) {
                queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
                queryClient.invalidateQueries({ queryKey: ['linkingEntries'] });
            }
        });

        setConnection(newConnection);

        return () => {
            newConnection.stop();
        };
    }, [organizationId, queryClient]);

    return connection;
};
            
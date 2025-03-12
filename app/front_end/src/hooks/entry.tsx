import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toastService from '@/services/toast';
import { createEntry, deleteEntry, deleteFile, downloadFile, getEntries, updateEntry, getLinkingEntries, getGraphEntries } from '@/services/api';


export const useGetEntries = (organizationId : string, groupId : string) => {
    return useQuery({
        queryKey: ['entries', organizationId],
        queryFn: () => getEntries(organizationId, groupId),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
};

export const useGetLinkingEntries = (organizationId : string, entryToExclude : string) => {
    return useQuery({
        queryKey: ['linkingEntries'],
        queryFn: () => getLinkingEntries(organizationId, entryToExclude),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
}

export const useGetGraphEntries = (organizationId : string) => {
    return useQuery({
        queryKey: ['graphEntries', organizationId],
        queryFn: () => getGraphEntries(organizationId),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
}

export const useDownloadFile = (organizationId : string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({groupId, entryId} : { groupId : string, entryId : string}) => downloadFile(groupId, entryId),
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("File downloaded successfully");
            queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
        },
    });
};

export const useCreateEntry = (organizationId : string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: createEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry created successfully");
            queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
            queryClient.invalidateQueries({ queryKey: ['linkingEntries'] });

        },
    });
}

export const useUpdateEntry = (organizationId : string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: updateEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry updated successfully");
            queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
            queryClient.invalidateQueries({ queryKey: ['linkingEntries'] });
        },
    });
}

export const useDeleteEntry = (organizationId : string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: deleteEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
            queryClient.invalidateQueries({ queryKey: ['linkingEntries'] });
        },
    });
}

export const useDeleteFile = (organizationId : string) => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({groupId, entryId} : { groupId : string, entryId : string}) => deleteFile(groupId, entryId),
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("File deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['entries', organizationId] });
        },
    });
}
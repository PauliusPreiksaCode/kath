import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import toastService from '@/services/toast';
import { createEntry, deleteEntry, deleteFile, downloadFile, getEntries, updateEntry } from '@/services/api';


export const useGetEntries = (organizationId : string, groupId : string) => {
    return useQuery({
        queryKey: ['entries'],
        queryFn: () => getEntries(organizationId, groupId),
        refetchOnWindowFocus: false,
        refetchInterval: false,
    });
};

export const useDownloadFile = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({groupId, entryId} : { groupId : string, entryId : string}) => downloadFile(groupId, entryId),
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("File downloaded successfully");
            queryClient.invalidateQueries({ queryKey: ['entries'] });
        },
    });
};

export const useCreateEntry = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: createEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry created successfully");
            queryClient.invalidateQueries({ queryKey: ['entries'] });
        },
    });
}

export const useUpdateEntry = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: updateEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry updated successfully");
            queryClient.invalidateQueries({ queryKey: ['entries'] });
        },
    });
}

export const useDeleteEntry = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: deleteEntry,
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("Entry deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['entries'] });
        },
    });
}

export const useDeleteFile = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({groupId, entryId} : { groupId : string, entryId : string}) => deleteFile(groupId, entryId),
        onSuccess: (e) => {
            if (e !== undefined) 
                toastService.success("File deleted successfully");
            queryClient.invalidateQueries({ queryKey: ['entries'] });
        },
    });
}
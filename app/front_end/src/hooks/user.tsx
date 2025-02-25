import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { register, getAvailableLicences, buyLicence, updatePayment, getUserLicences, getUsers, transferLicence } from '@/services/api';
import { useNavigate } from 'react-router';


export const useRegister = () => {
  const navigate = useNavigate();
  
  return useMutation({
    mutationFn: register,
    onSuccess: () => {
      navigate('/login');
    },
  });
}

export const useGetLicences = () => {
  return useQuery({
    queryKey: ['licences'],
    queryFn: getAvailableLicences,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
};

export const useBuyLicence = () => {  

  return useMutation({
    mutationFn: buyLicence,
    onSuccess: () => {
    }, 
  });
}

export const useUpdatePayment = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updatePayment,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userLicences'] });
    },
  });
}

export const useGetUserLicences = () => {
  return useQuery({
    queryKey: ['userLicences'],
    queryFn: getUserLicences,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
}

export const useGetUsers = () => {
  return useQuery({
    queryKey: ['users'],
    queryFn: getUsers,
    refetchOnWindowFocus: false,
    refetchInterval: false,
  });
}

export const useTransferLicence = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: transferLicence,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userLicences'] });
    },
  });
}
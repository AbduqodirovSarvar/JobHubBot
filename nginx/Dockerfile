FROM nginx

COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY /etc/letsencrypt /etc/letsencrypt

EXPOSE 80
EXPOSE 443

CMD ["nginx", "-g", "daemon off;"]
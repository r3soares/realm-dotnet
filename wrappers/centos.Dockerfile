FROM centos:7

# Install EPEL & devtoolset
RUN yum install -y \
        epel-release \
        centos-release-scl-rh \
    && yum-config-manager --enable rhel-server-rhscl-7-rpms

RUN yum install -y \
        devtoolset-9 \
        rh-git218 \
        zlib-devel

RUN mkdir -p /opt/cmake && \
    curl -s https://cmake.org/files/v3.17/cmake-3.17.2-Linux-x86_64.sh -o /cmake.sh && \
    sh /cmake.sh --prefix=/opt/cmake --skip-license && \
    rm /cmake.sh

ENV PATH /opt/rh/rh-git218/root/usr/bin:/opt/rh/devtoolset-9/root/usr/bin:/opt/cmake/bin:$PATH
ENV LD_LIBRARY_PATH /opt/rh/devtoolset-9/root/usr/lib64:/opt/rh/devtoolset-9/root/usr/lib:/opt/rh/devtoolset-9/root/usr/lib64/dyninst:/opt/rh/devtoolset-9/root/usr/lib/dyninst:/opt/rh/devtoolset-9/root/usr/lib64:/opt/rh/devtoolset-9/root/usr/lib

VOLUME /source
CMD ["/source/build.sh"]
